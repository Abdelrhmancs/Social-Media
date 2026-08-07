using API.Domain.Entites;
using System;
using System.Collections.Generic;

namespace Application.DTOs.ChatDTOs
{
    public class MessageDto
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public string SenderId { get; set; } = null!;
        public string SenderName { get; set; } = null!;
        public string? SenderPic { get; set; }
        public string Content { get; set; } = null!;
        public MessageType Type { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsDeleted { get; set; }
        public List<ReadReceiptDto> ReadReceipts { get; set; } = new();
    }
}
