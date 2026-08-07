using Application.DTOs.ChatDTOs;
using Application.Interfaces;
using Domain.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.UseCases.Chat
{
    public class GetUserConversationsUseCase
    {
        private readonly IConversationRepository _conversationRepo;
        private readonly IUserContext _userContext;

        public GetUserConversationsUseCase(IConversationRepository conversationRepo, IUserContext userContext)
        {
            _conversationRepo = conversationRepo;
            _userContext = userContext;
        }

        public async Task<ResultT<List<ConversationDto>>> ExecuteAsync()
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return ResultT<List<ConversationDto>>.Failure(new List<string> { "Unauthorized" }, ErrorType.Unauthorized);

            return await _conversationRepo.GetUserConversationsAsync(userId);
        }
    }
}
