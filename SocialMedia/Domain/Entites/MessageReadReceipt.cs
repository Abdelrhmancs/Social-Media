using System;

namespace API.Domain.Entites;

public class MessageReadReceipt
{
    public long Id { get; set; }
    public long MessageId { get; set; }
    public string UserId { get; set; } = null!;
    public DateTime ReadAt { get; set; }

    public virtual Message Message { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
