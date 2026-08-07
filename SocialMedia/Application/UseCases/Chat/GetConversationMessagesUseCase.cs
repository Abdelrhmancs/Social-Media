using Application.DTOs.ChatDTOs;
using Application.Interfaces;
using Domain.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.UseCases.Chat
{
    public class GetConversationMessagesUseCase
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IConversationRepository _conversationRepo;
        private readonly IUserContext _userContext;

        public GetConversationMessagesUseCase(
            IMessageRepository messageRepo,
            IConversationRepository conversationRepo,
            IUserContext userContext)
        {
            _messageRepo = messageRepo;
            _conversationRepo = conversationRepo;
            _userContext = userContext;
        }

        public async Task<ResultT<List<MessageDto>>> ExecuteAsync(long conversationId, int page = 1, int pageSize = 30)
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return ResultT<List<MessageDto>>.Failure(new List<string> { "Unauthorized" }, ErrorType.Unauthorized);

            var isMember = await _conversationRepo.IsUserMemberAsync(conversationId, userId);
            if (!isMember)
                return ResultT<List<MessageDto>>.Failure(new List<string> { "You are not a member of this conversation" }, ErrorType.Forbidden);

            return await _messageRepo.GetConversationMessagesAsync(conversationId, userId, page, pageSize);
        }
    }
}
