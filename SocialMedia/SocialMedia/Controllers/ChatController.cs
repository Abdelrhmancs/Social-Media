using Application.DTOs.ChatDTOs;
using Application.UseCases.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly CreateDirectConversationUseCase _createDirectUseCase;
        private readonly CreateGroupConversationUseCase _createGroupUseCase;
        private readonly GetUserConversationsUseCase _getConversationsUseCase;
        private readonly SendMessageUseCase _sendMessageUseCase;
        private readonly GetConversationMessagesUseCase _getMessagesUseCase;
        private readonly MarkMessageAsReadUseCase _markReadUseCase;
        private readonly GetMessageReadReceiptsUseCase _getReceiptsUseCase;
        private readonly AddGroupMemberUseCase _addMemberUseCase;
        private readonly RemoveGroupMemberUseCase _removeMemberUseCase;

        public ChatController(
            CreateDirectConversationUseCase createDirectUseCase,
            CreateGroupConversationUseCase createGroupUseCase,
            GetUserConversationsUseCase getConversationsUseCase,
            SendMessageUseCase sendMessageUseCase,
            GetConversationMessagesUseCase getMessagesUseCase,
            MarkMessageAsReadUseCase markReadUseCase,
            GetMessageReadReceiptsUseCase getReceiptsUseCase,
            AddGroupMemberUseCase addMemberUseCase,
            RemoveGroupMemberUseCase removeMemberUseCase)
        {
            _createDirectUseCase = createDirectUseCase;
            _createGroupUseCase = createGroupUseCase;
            _getConversationsUseCase = getConversationsUseCase;
            _sendMessageUseCase = sendMessageUseCase;
            _getMessagesUseCase = getMessagesUseCase;
            _markReadUseCase = markReadUseCase;
            _getReceiptsUseCase = getReceiptsUseCase;
            _addMemberUseCase = addMemberUseCase;
            _removeMemberUseCase = removeMemberUseCase;
        }

        [HttpPost("direct")]
        public async Task<IActionResult> CreateDirectConversation([FromBody] CreateDirectConversationDto dto)
        {
            var result = await _createDirectUseCase.ExecuteAsync(dto.TargetUserId);
            if (result.IsSuccess) return Ok(result.Data);
            return BadRequest(result.Errors);
        }

        [HttpPost("group")]
        public async Task<IActionResult> CreateGroupConversation([FromBody] CreateGroupConversationDto dto)
        {
            var result = await _createGroupUseCase.ExecuteAsync(dto);
            if (result.IsSuccess) return Ok(result.Data);
            return BadRequest(result.Errors);
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var result = await _getConversationsUseCase.ExecuteAsync();
            if (result.IsSuccess) return Ok(result.Data);
            return BadRequest(result.Errors);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            var result = await _sendMessageUseCase.ExecuteAsync(dto);
            if (result.IsSuccess) return Ok(result.Data);
            return BadRequest(result.Errors);
        }

        [HttpGet("conversations/{id}/messages")]
        public async Task<IActionResult> GetMessages(long id, [FromQuery] int page = 1, [FromQuery] int pageSize = 30)
        {
            var result = await _getMessagesUseCase.ExecuteAsync(id, page, pageSize);
            if (result.IsSuccess) return Ok(result.Data);
            return BadRequest(result.Errors);
        }

        [HttpPost("messages/{messageId}/read/{conversationId}")]
        public async Task<IActionResult> MarkMessageAsRead(long messageId, long conversationId)
        {
            var result = await _markReadUseCase.ExecuteAsync(messageId, conversationId);
            if (result.IsSuccess) return Ok(result.Message);
            return BadRequest(result.Errors);
        }

        [HttpGet("messages/{messageId}/read-receipts")]
        public async Task<IActionResult> GetMessageReadReceipts(long messageId)
        {
            var result = await _getReceiptsUseCase.ExecuteAsync(messageId);
            if (result.IsSuccess) return Ok(result.Data);
            return BadRequest(result.Errors);
        }

        [HttpPost("group/{id}/members")]
        public async Task<IActionResult> AddGroupMember(long id, [FromBody] string userId)
        {
            var result = await _addMemberUseCase.ExecuteAsync(id, userId);
            if (result.IsSuccess) return Ok(result.Message);
            return BadRequest(result.Errors);
        }

        [HttpDelete("group/{id}/members/{userId}")]
        public async Task<IActionResult> RemoveGroupMember(long id, string userId)
        {
            var result = await _removeMemberUseCase.ExecuteAsync(id, userId);
            if (result.IsSuccess) return Ok(result.Message);
            return BadRequest(result.Errors);
        }
    }
}
