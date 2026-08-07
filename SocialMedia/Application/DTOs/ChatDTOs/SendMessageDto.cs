using API.Domain.Entites;

namespace Application.DTOs.ChatDTOs
{
    public class SendMessageDto
    {
        public long ConversationId { get; set; }
        public string Content { get; set; } = null!;
        public MessageType Type { get; set; } = MessageType.Text;
    }
}
