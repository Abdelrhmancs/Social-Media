using Application.DTOs.ChatDTOs;
using Application.Interfaces;
using Domain.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.UseCases.Chat
{
    public class CreateGroupConversationUseCase
    {
        private readonly IConversationRepository _conversationRepo;
        private readonly IUserContext _userContext;

        public CreateGroupConversationUseCase(IConversationRepository conversationRepo, IUserContext userContext)
        {
            _conversationRepo = conversationRepo;
            _userContext = userContext;
        }

        public async Task<ResultT<ConversationDto>> ExecuteAsync(CreateGroupConversationDto dto)
        {
            var currentUserId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return ResultT<ConversationDto>.Failure(new List<string> { "Unauthorized" }, ErrorType.Unauthorized);

            if (string.IsNullOrWhiteSpace(dto.Name))
                return ResultT<ConversationDto>.Failure(new List<string> { "Group name is required" });

            return await _conversationRepo.CreateGroupConversationAsync(currentUserId, dto);
        }
    }
}
