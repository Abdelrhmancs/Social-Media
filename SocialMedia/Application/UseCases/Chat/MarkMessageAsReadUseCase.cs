using Application.Interfaces;
using Domain.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.Chat
{
    public class MarkMessageAsReadUseCase
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IChatHubNotifier _hubNotifier;
        private readonly IUserContext _userContext;

        public MarkMessageAsReadUseCase(
            IMessageRepository messageRepo,
            IChatHubNotifier hubNotifier,
            IUserContext userContext)
        {
            _messageRepo = messageRepo;
            _hubNotifier = hubNotifier;
            _userContext = userContext;
        }

        public async Task<Result> ExecuteAsync(long messageId, long conversationId)
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(new List<string> { "Unauthorized" });

            var result = await _messageRepo.MarkMessageAsReadAsync(messageId, userId);
            if (!result.IsSuccess)
                return result;

            // Fetch the receipt just created and broadcast to conversation group
            var receiptsResult = await _messageRepo.GetMessageReadReceiptsAsync(messageId, userId);
            if (receiptsResult.IsSuccess)
            {
                var myReceipt = receiptsResult.Data.FirstOrDefault(r => r.UserId == userId);
                if (myReceipt != null)
                    await _hubNotifier.NotifyMessageReadAsync(conversationId, messageId, myReceipt);
            }

            return result;
        }
    }
}
