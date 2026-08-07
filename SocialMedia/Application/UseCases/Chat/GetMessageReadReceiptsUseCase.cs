using Application.DTOs.ChatDTOs;
using Application.Interfaces;
using Domain.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.UseCases.Chat
{
    public class GetMessageReadReceiptsUseCase
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IUserContext _userContext;

        public GetMessageReadReceiptsUseCase(IMessageRepository messageRepo, IUserContext userContext)
        {
            _messageRepo = messageRepo;
            _userContext = userContext;
        }

        public async Task<ResultT<List<ReadReceiptDto>>> ExecuteAsync(long messageId)
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return ResultT<List<ReadReceiptDto>>.Failure(new List<string> { "Unauthorized" }, ErrorType.Unauthorized);

            return await _messageRepo.GetMessageReadReceiptsAsync(messageId, userId);
        }
    }
}
