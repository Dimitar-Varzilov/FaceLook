using AutoMapper;
using AutoMapper.QueryableExtensions;
using FaceLook.Data;
using FaceLook.Data.Entities;
using FaceLook.Services.Exceptions;
using FaceLook.Services.Interfaces;
using FaceLook.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FaceLook.Services.Core
{
    public class ChatService(ApplicationDbContext dbContext, UserManager<User> userManager, IMapper mapper) : IChatService
    {
        public async Task<ChatViewModel?> GetChatByIdAsync(Guid chatId)
        {
            if (chatId == Guid.Empty)
                throw new ValidationException($"Parameter {nameof(chatId)} in {nameof(GetChatByIdAsync)} is empty");

            return await dbContext.Chats
                .AsNoTracking()
                .Include(chat => chat.Messages)
                .ProjectTo<ChatViewModel>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(chat => chat.Id == chatId);
        }

        public async Task<IEnumerable<ChatViewModel>> GetUserChatsAsync(string? userId)
        {
            ArgumentException.ThrowIfNullOrEmpty(userId);

            var userExists = await userManager.FindByIdAsync(userId) ?? throw new ResourceNotFoundException("User not found");

            return await dbContext.Chats
                .AsNoTracking()
                .Include(chat => chat.Messages)
                .Where(chat => chat.ParticipantIds.Contains(userExists.Id))
                .ProjectTo<ChatViewModel>(mapper.ConfigurationProvider)
                .ToArrayAsync();
        }
        public async Task<ChatViewModel> CreateChatAsync(string userId, ISet<string> participants)
        {
            ArgumentException.ThrowIfNullOrEmpty(userId);
            if (participants.Count < 2)
                throw new ValidationException("Atleast two participants are required for chat");
            else if (!participants.Contains(userId))
                throw new ValidationException("Requester is not participant");

            var existingChat = await FindChatByParticipantsAsync(participants);
            if (existingChat != null)
                return existingChat;

            var newChat = new Chat()
            {
                CreatedAt = DateTimeOffset.Now,
                Id = Guid.NewGuid(),
                IsDeleted = false,
                ModifiedBy = userId,
                Name = string.Empty,
                ParticipantIds = [.. participants],
                MessageIds = []
            };

            var res = await dbContext.AddAsync(newChat);
            await dbContext.SaveChangesAsync();

            return mapper.Map<ChatViewModel>(res.Entity);
        }

        private async Task<ChatViewModel?> FindChatByParticipantsAsync(ISet<string> participants)
        {
            var participantList = participants.OrderBy(p => p).ToList();

            var existingChats = await dbContext.Chats
                .AsNoTracking()
                .Where(chat => chat.ParticipantIds.Count == participants.Count)
                .ProjectTo<ChatViewModel>(mapper.ConfigurationProvider)
                .ToArrayAsync();

            foreach (var chat in existingChats)
            {
                var chatParticipants = chat.ParticipantIds.OrderBy(p => p).ToList();
                if (chatParticipants.SequenceEqual(participantList))
                    return chat;
            }

            return null;
        }

        public async Task<bool> UpdateChatNameAsync(string userId, Guid chatId, string name)
        {
            ArgumentException.ThrowIfNullOrEmpty(userId);
            if (chatId == Guid.Empty)
                throw new ValidationException($"Parameter {nameof(chatId)} is empty");

            var chat = await dbContext.Chats
                .FirstOrDefaultAsync(c => c.Id == chatId) ?? throw new ResourceNotFoundException("Chat not found");

            if (!chat.ParticipantIds.Contains(userId))
                throw new ValidationException("User is not a participant of this chat");

            chat.Name = name ?? string.Empty;
            chat.ModifiedBy = userId;
            return await dbContext.SaveChangesAsync() > 0;
        }
    }
}
