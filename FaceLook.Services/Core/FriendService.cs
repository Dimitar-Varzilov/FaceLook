using FaceLook.Common.Enums;
using FaceLook.Data;
using FaceLook.Data.Entities;
using FaceLook.Services.Exceptions;
using FaceLook.Services.Interfaces;
using FaceLook.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FaceLook.Services.Core
{
    public class FriendService(ApplicationDbContext dbContext, UserManager<User> userManager) : IFriendService
    {
        public async Task<FriendsPageViewModel> GetFriendsPageAsync(string currentUserId)
        {
            var allFriendships = await dbContext.Friendships
                .AsNoTracking()
                .Include(f => f.Requester)
                .Include(f => f.Addressee)
                .Where(f => f.RequesterId == currentUserId || f.AddresseeId == currentUserId)
                .ToListAsync();

            return new FriendsPageViewModel
            {
                AcceptedFriends = MapFriendships(allFriendships.Where(f => f.Status == FriendshipStatus.Accepted), currentUserId),
                PendingRequests = MapFriendships(allFriendships.Where(f => f.Status == FriendshipStatus.Pending && f.AddresseeId == currentUserId), currentUserId),
                SentRequests = MapFriendships(allFriendships.Where(f => f.Status == FriendshipStatus.Pending && f.RequesterId == currentUserId), currentUserId),
                SendRequest = new SendFriendRequestViewModel { AddresseeEmail = string.Empty }
            };
        }

        public async Task<IEnumerable<FriendViewModel>> GetAcceptedFriendsAsync(string userId)
        {
            var friendships = await dbContext.Friendships
                .AsNoTracking()
                .Include(f => f.Requester)
                .Include(f => f.Addressee)
                .Where(f => (f.RequesterId == userId || f.AddresseeId == userId) && f.Status == FriendshipStatus.Accepted)
                .ToListAsync();

            return MapFriendships(friendships, userId);
        }

        public async Task<FriendViewModel> SendFriendRequestAsync(string requesterId, string addresseeEmail)
        {
            var requester = await userManager.FindByIdAsync(requesterId)
                ?? throw new ResourceNotFoundException("Requester user not found");

            var addressee = await userManager.FindByEmailAsync(addresseeEmail)
                ?? throw new ResourceNotFoundException($"No user found with email '{addresseeEmail}'");

            if (requester.Id == addressee.Id)
                throw new ValidationException("You cannot send a friend request to yourself");

            var existing = await dbContext.Friendships.FirstOrDefaultAsync(f =>
                (f.RequesterId == requesterId && f.AddresseeId == addressee.Id) ||
                (f.RequesterId == addressee.Id && f.AddresseeId == requesterId));

            if (existing != null)
            {
                if (existing.Status == FriendshipStatus.Accepted)
                    throw new ValidationException("You are already friends with this user");

                if (existing.Status == FriendshipStatus.Pending)
                    throw new ValidationException("A friend request is already pending with this user");

                if (existing.Status == FriendshipStatus.Blocked)
                    throw new ValidationException("This user has blocked you. You cannot send them a friend request");

                if (existing.Status == FriendshipStatus.Declined)
                {
                    // Reuse the existing record — update it back to Pending
                    // regardless of which direction the original request was,
                    // the current sender becomes the new requester.
                    existing.RequesterId = requesterId;
                    existing.AddresseeId = addressee.Id;
                    existing.Status = FriendshipStatus.Pending;
                    existing.ModifiedAt = DateTimeOffset.UtcNow;
                    existing.ModifiedBy = requesterId;

                    dbContext.Friendships.Update(existing);
                    await dbContext.SaveChangesAsync();

                    return MapFriendship(existing, addressee, isRequester: true);
                }
            }

            var friendship = new Friendship()
            {
                Id = Guid.NewGuid(),
                RequesterId = requesterId,
                AddresseeId = addressee.Id,
                Status = FriendshipStatus.Pending,
                CreatedAt = DateTimeOffset.UtcNow,
                IsDeleted = false,
                ModifiedBy = requesterId
            };

            await dbContext.Friendships.AddAsync(friendship);
            await dbContext.SaveChangesAsync();

            return MapFriendship(friendship, addressee, isRequester: true);
        }

        public async Task<FriendViewModel> AcceptFriendRequestAsync(string currentUserId, Guid friendshipId)
        {
            var friendship = await GetFriendshipForAddresseeAsync(currentUserId, friendshipId);

            if (friendship.Status != FriendshipStatus.Pending)
                throw new ValidationException("Only pending requests can be accepted");

            friendship.Status = FriendshipStatus.Accepted;
            friendship.ModifiedAt = DateTimeOffset.UtcNow;

            dbContext.Friendships.Update(friendship);
            await dbContext.SaveChangesAsync();

            var friend = friendship.Requester;
            return MapFriendship(friendship, friend, isRequester: false);
        }

        public async Task<FriendViewModel> DeclineFriendRequestAsync(string currentUserId, Guid friendshipId)
        {
            var friendship = await GetFriendshipForAddresseeAsync(currentUserId, friendshipId);

            if (friendship.Status != FriendshipStatus.Pending)
                throw new ValidationException("Only pending requests can be declined");

            friendship.Status = FriendshipStatus.Declined;
            friendship.ModifiedAt = DateTimeOffset.UtcNow;

            dbContext.Friendships.Update(friendship);
            await dbContext.SaveChangesAsync();

            var friend = friendship.Requester;
            return MapFriendship(friendship, friend, isRequester: false);
        }

        public async Task<bool> RemoveFriendAsync(string currentUserId, Guid friendshipId)
        {
            var friendship = await dbContext.Friendships
                .FirstOrDefaultAsync(f => f.Id == friendshipId &&
                    (f.RequesterId == currentUserId || f.AddresseeId == currentUserId))
                ?? throw new ResourceNotFoundException("Friendship not found");

            dbContext.Friendships.Remove(friendship);
            return await dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> BlockUserAsync(string currentUserId, Guid friendshipId)
        {
            var friendship = await dbContext.Friendships
                .FirstOrDefaultAsync(f => f.Id == friendshipId &&
                    (f.RequesterId == currentUserId || f.AddresseeId == currentUserId))
                ?? throw new ResourceNotFoundException("Friendship not found");

            friendship.Status = FriendshipStatus.Blocked;
            friendship.ModifiedAt = DateTimeOffset.UtcNow;

            dbContext.Friendships.Update(friendship);
            return await dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> AreFriendsAsync(string userId1, string userId2)
        {
            return await dbContext.Friendships.AnyAsync(f =>
                ((f.RequesterId == userId1 && f.AddresseeId == userId2) ||
                 (f.RequesterId == userId2 && f.AddresseeId == userId1)) &&
                f.Status == FriendshipStatus.Accepted);
        }

        private async Task<Friendship> GetFriendshipForAddresseeAsync(string addresseeId, Guid friendshipId)
        {
            return await dbContext.Friendships
                .Include(f => f.Requester)
                .Include(f => f.Addressee)
                .FirstOrDefaultAsync(f => f.Id == friendshipId && f.AddresseeId == addresseeId)
                ?? throw new ResourceNotFoundException("Friend request not found");
        }

        private static IEnumerable<FriendViewModel> MapFriendships(IEnumerable<Friendship> friendships, string currentUserId)
        {
            return friendships.Select(f =>
            {
                bool isRequester = f.RequesterId == currentUserId;
                var friend = isRequester ? f.Addressee : f.Requester;
                return MapFriendship(f, friend, isRequester);
            });
        }

        private static FriendViewModel MapFriendship(Friendship f, User friend, bool isRequester)
        {
            return new FriendViewModel
            {
                FriendshipId = f.Id,
                FriendId = friend.Id,
                FriendEmail = friend.Email ?? string.Empty,
                FriendUserName = friend.UserName ?? friend.Email ?? string.Empty,
                Status = f.Status,
                CreatedAt = f.CreatedAt,
                IsRequester = isRequester
            };
        }
    }
}
