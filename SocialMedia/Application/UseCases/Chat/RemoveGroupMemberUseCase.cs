using Application.Interfaces;
using Domain.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.UseCases.Chat
{
    public class RemoveGroupMemberUseCase
    {
        private readonly IConversationRepository _conversationRepo;
        private readonly IUserContext _userContext;

        public RemoveGroupMemberUseCase(IConversationRepository conversationRepo, IUserContext userContext)
        {
            _conversationRepo = conversationRepo;
            _userContext = userContext;
        }

        public async Task<Result> ExecuteAsync(long conversationId, string userIdToRemove)
        {
            var currentUserId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Result.Failure(new List<string> { "Unauthorized" });

            return await _conversationRepo.RemoveMemberAsync(conversationId, userIdToRemove, currentUserId);
        }
    }
}
