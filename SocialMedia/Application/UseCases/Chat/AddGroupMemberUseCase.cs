using Application.Interfaces;
using Domain.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.UseCases.Chat
{
    public class AddGroupMemberUseCase
    {
        private readonly IConversationRepository _conversationRepo;
        private readonly IUserContext _userContext;

        public AddGroupMemberUseCase(IConversationRepository conversationRepo, IUserContext userContext)
        {
            _conversationRepo = conversationRepo;
            _userContext = userContext;
        }

        public async Task<Result> ExecuteAsync(long conversationId, string userIdToAdd)
        {
            var currentUserId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Result.Failure(new List<string> { "Unauthorized" });

            return await _conversationRepo.AddMemberAsync(conversationId, userIdToAdd, currentUserId);
        }
    }
}
