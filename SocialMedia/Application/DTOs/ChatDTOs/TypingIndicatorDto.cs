namespace Application.DTOs.ChatDTOs
{
    public class TypingIndicatorDto
    {
        public long ConversationId { get; set; }
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public bool IsTyping { get; set; }
    }
}
