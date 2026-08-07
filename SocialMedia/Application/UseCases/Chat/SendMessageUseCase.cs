using Application.DTOs.ChatDTOs;
using Application.Interfaces;
using Domain.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.UseCases.Chat
{
    public class SendMessageUseCase
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IConversationRepository _conversationRepo;
        private readonly IChatHubNotifier _hubNotifier;
        private readonly IUserContext _userContext;

        public SendMessageUseCase(
            IMessageRepository messageRepo,
            IConversationRepository conversationRepo,
            IChatHubNotifier hubNotifier,
            IUserContext userContext)
        {
            _messageRepo = messageRepo;
            _conversationRepo = conversationRepo;
            _hubNotifier = hubNotifier;
            _userContext = userContext;
        }

        public async Task<ResultT<MessageDto>> ExecuteAsync(SendMessageDto dto)
        {
            var senderId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(senderId))
                return ResultT<MessageDto>.Failure(new List<string> { "Unauthorized" }, ErrorType.Unauthorized);

            var isMember = await _conversationRepo.IsUserMemberAsync(dto.ConversationId, senderId);
            if (!isMember)
                return ResultT<MessageDto>.Failure(new List<string> { "You are not a member of this conversation" }, ErrorType.Forbidden);

            var result = await _messageRepo.SendMessageAsync(senderId, dto);
            if (!result.IsSuccess)
                return result;

            // Broadcast to all members via SignalR
            await _hubNotifier.SendMessageToConversationAsync(dto.ConversationId, result.Data);
            return result;
        }
    }
}
