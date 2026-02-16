using AutoMapper;
using AutoMapper.QueryableExtensions;
using FaceLook.Data;
using FaceLook.Data.Entities;
using FaceLook.Enums;
using FaceLook.Services.Exceptions;
using FaceLook.Services.Hubs;
using FaceLook.ViewModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FaceLook.Services
{
    public class MessageService(ApplicationDbContext dbContext, IMapper mapper, IHubContext<ChatHub, IChatClient> hubContext, IUserService userService) : IMessageService
    {

        public async Task<MessageViewModel?> GetMessageById(Guid messageId)
        {
            return await dbContext.Messages
                .AsNoTracking()
                .Where(m => m.Id == messageId && !m.IsDeleted)
                .OrderByDescending(m => m.CreatedAt)
                .ProjectTo<MessageViewModel>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<MessageViewModel>> GetUserMessagesAsync(string userId)
        {
            return await dbContext.Messages
                .AsNoTracking()
                .Where(m => (m.SenderId == userId || m.ReceiverId == userId) && !m.IsDeleted)
                .OrderByDescending(m => m.CreatedAt)
                .ProjectTo<MessageViewModel>(mapper.ConfigurationProvider)
                .ToArrayAsync();
        }

        public async Task<MessageViewModel> SendMessageAsync(SendMessageRequest sendMessageRequest)
        {
            var sender = await GetValidatedMessageAndUserAsync(sendMessageRequest.Content, sendMessageRequest.SenderId);

            if (string.Equals(sender.Email, sendMessageRequest.ReceiverEmail, StringComparison.OrdinalIgnoreCase))
                throw new ValidationException("Cannot send message to self");

            var receiver = await userService.GetUserByEmailAsync(sendMessageRequest.ReceiverEmail);
            if (receiver == null)
                throw new ResourceNotFoundException(nameof(receiver));

            var messageToAdd = new Message()
            {
                Content = sendMessageRequest.Content,
                CreatedAt = DateTimeOffset.Now,
                Id = Guid.NewGuid(),
                IsDeleted = false,
                MessageStatus = MessageStatus.New,
                ModifiedBy = sender.Id,
                ReceiverId = receiver.Id,
                SenderId = sender.Id,
            };

            var addedEntityEntry = await dbContext.Messages.AddAsync(messageToAdd);
            await SaveChangesAndSendMessageAsync(
                senderEmail: sender.Email,
                receiverEmail: receiver.Email,
                content: addedEntityEntry.Entity.Content);

            return mapper.Map<MessageViewModel>(addedEntityEntry.Entity);
        }

        public async Task<MessageViewModel> UpdateMessageAsync(string userId, MessageViewModel request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                throw new ValidationException("Message is required");

            if (request.SenderId != userId)
                throw new ValidationException("User can only update own messages");

            var messageToUpdate = await dbContext.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync(m => m.Id == request.Id) ?? throw new ResourceNotFoundException("Message is not found");

            if (messageToUpdate.IsDeleted)
                throw new ValidationException("Cannot edit deleted messages");

            messageToUpdate.ModifiedAt = DateTimeOffset.Now;
            messageToUpdate.ModifiedBy = messageToUpdate.SenderId;
            messageToUpdate.Content = request.Content;

            var updatedEntityEntry = dbContext.Messages.Update(messageToUpdate);
            await SaveChangesAndSendMessageAsync(
                senderEmail: messageToUpdate.Sender.Email,
                receiverEmail: messageToUpdate.Receiver.Email,
                content: updatedEntityEntry.Entity.Content);

            return mapper.Map<MessageViewModel>(updatedEntityEntry.Entity);
        }

        public async Task<bool> DeleteMessageAsync(string userId, Guid messageId)
        {
            var messageToDelete = await dbContext.Messages
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == messageId);
            if (messageToDelete is null)
            {
                return false;
            }
            else if (messageToDelete.SenderId != userId)
            {
                throw new ValidationException("User can only delete own messages");
            }

            dbContext.Messages.Remove(messageToDelete);

            return await dbContext.SaveChangesAsync() > 0;
        }

        private async Task<User> GetValidatedMessageAndUserAsync(string? message, string userId)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ValidationException("Message is required");

            var sender = await userService.GetUserByIdAsync(userId);
            if (sender == null)
                throw new ResourceNotFoundException(nameof(sender));

            return sender;
        }

        private async Task SaveChangesAndSendMessageAsync(string? senderEmail, string? receiverEmail, string content)
        {
            int rowModifiedCount = await dbContext.SaveChangesAsync();

            if (rowModifiedCount > 0 && senderEmail is not null && receiverEmail is not null && !string.IsNullOrEmpty(content))
            {
                await hubContext.Clients.Group(receiverEmail).ReceiveMessage(senderEmail, content);
            }
        }
    }
}
