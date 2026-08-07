using System;
using System.Collections.Generic;

namespace API.Domain.Entites;

public class Message
{
    public long Id { get; set; }
    public long ConversationId { get; set; }
    public string SenderId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public MessageType Type { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsDeleted { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;
    public virtual User Sender { get; set; } = null!;
    public virtual ICollection<MessageReadReceipt> ReadReceipts { get; set; } = new List<MessageReadReceipt>();
}

public enum MessageType
{
    Text = 0,
    Image = 1,
    Video = 2,
    File = 3
}
