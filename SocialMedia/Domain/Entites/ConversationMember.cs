using System;

namespace API.Domain.Entites;

public class ConversationMember
{
    public long Id { get; set; }
    public long ConversationId { get; set; }
    public string UserId { get; set; } = null!;
    public MemberRole Role { get; set; }
    public DateTime JoinedAt { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}

public enum MemberRole
{
    Member = 0,
    Admin = 1
}
