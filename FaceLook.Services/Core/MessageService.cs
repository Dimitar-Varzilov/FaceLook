using AutoMapper;
using AutoMapper.QueryableExtensions;
using FaceLook.Common.Enums;
using FaceLook.Data;
using FaceLook.Data.Entities;
using FaceLook.Services.Exceptions;
using FaceLook.Services.Hubs;
using FaceLook.Services.Interfaces;
using FaceLook.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FaceLook.Services.Core
{
    public class MessageService(ApplicationDbContext dbContext, IMapper mapper, IHubContext<ChatHub, IChatClient> hubContext, UserManager<User> userManager) : IMessageService
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
                .Include(m => m.Chat)
                .Where(m => m.Chat.ParticipantIds.Contains(userId) && !m.IsDeleted)
                .OrderByDescending(m => m.CreatedAt)
                .ProjectTo<MessageViewModel>(mapper.ConfigurationProvider)
                .ToArrayAsync();
        }

        public async Task<MessageViewModel> SendMessageAsync(SendMessageRequest sendMessageRequest)
        {
            var sender = await GetValidatedMessageAndUserAsync(sendMessageRequest.Content, sendMessageRequest.SenderId);

            if (sendMessageRequest.ChatId == Guid.Empty)
                throw new ValidationException("ChatId is required");

            var chat = await dbContext.Chats
                .FirstOrDefaultAsync(c => c.Id == sendMessageRequest.ChatId)
                ?? throw new ResourceNotFoundException("Chat not found");

            if (!chat.ParticipantIds.Contains(sender.Id))
                throw new ValidationException("User is not a participant of this chat");

            var messageToAdd = new Message()
            {
                Content = sendMessageRequest.Content,
                ChatId = sendMessageRequest.ChatId,
                CreatedAt = DateTimeOffset.Now,
                Id = Guid.NewGuid(),
                IsDeleted = false,
                MessageStatus = MessageStatus.New,
                ModifiedBy = sender.Id,
                SenderId = sender.Id,
            };

            var addedEntityEntry = await dbContext.Messages.AddAsync(messageToAdd);

            // Add message ID to chat's MessageIds collection
            chat.MessageIds.Add(messageToAdd.Id);
            dbContext.Chats.Update(chat);

            await SaveChangesAndSendChatMessageAsync(sendMessageRequest.ChatId, sender.Email, messageToAdd.Content);

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
                .Include(m => m.Chat)
                .FirstOrDefaultAsync(m => m.Id == request.Id) ?? throw new ResourceNotFoundException("Message is not found");

            if (messageToUpdate.IsDeleted)
                throw new ValidationException("Cannot edit deleted messages");

            messageToUpdate.ModifiedAt = DateTimeOffset.Now;
            messageToUpdate.ModifiedBy = messageToUpdate.SenderId;
            messageToUpdate.Content = request.Content;

            var updatedEntityEntry = dbContext.Messages.Update(messageToUpdate);
            await SaveChangesAndSendChatMessageAsync(messageToUpdate.ChatId, messageToUpdate.Sender.Email, updatedEntityEntry.Entity.Content);

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

            var sender = await userManager.FindByIdAsync(userId);
            if (sender == null)
                throw new ResourceNotFoundException(nameof(sender));

            return sender;
        }

        private async Task SaveChangesAndSendChatMessageAsync(Guid chatId, string? senderEmail, string content)
        {
            int rowModifiedCount = await dbContext.SaveChangesAsync();

            if (rowModifiedCount > 0 && senderEmail is not null && !string.IsNullOrEmpty(content))
            {
                // Notify all chat participants
                var chatParticipants = await dbContext.Chats
                    .AsNoTracking()
                    .Where(c => c.Id == chatId)
                    .SelectMany(c => c.Participants.Select(p => p.Email))
                    .Where(ce => ce != null)
                    .Cast<string>()
                    .ToListAsync();

                foreach (var participantEmail in chatParticipants)
                {
                    await hubContext.Clients.Group(participantEmail).ReceiveMessage(senderEmail, content);
                }
            }
        }
    }
}
