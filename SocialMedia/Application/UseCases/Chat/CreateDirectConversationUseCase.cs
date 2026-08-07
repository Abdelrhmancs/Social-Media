using Application.DTOs.ChatDTOs;
using Application.Interfaces;
using Domain.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.UseCases.Chat
{
    public class CreateDirectConversationUseCase
    {
        private readonly IConversationRepository _conversationRepo;
        private readonly IUserContext _userContext;

        public CreateDirectConversationUseCase(IConversationRepository conversationRepo, IUserContext userContext)
        {
            _conversationRepo = conversationRepo;
            _userContext = userContext;
        }

        public async Task<ResultT<ConversationDto>> ExecuteAsync(string targetUserId)
        {
            var currentUserId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return ResultT<ConversationDto>.Failure(new List<string> { "Unauthorized" }, ErrorType.Unauthorized);

            if (currentUserId == targetUserId)
                return ResultT<ConversationDto>.Failure(new List<string> { "Cannot create a conversation with yourself" });

            return await _conversationRepo.CreateDirectConversationAsync(currentUserId, targetUserId);
        }
    }
}
