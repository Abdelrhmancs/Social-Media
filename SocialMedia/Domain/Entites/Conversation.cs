using System;
using System.Collections.Generic;

namespace API.Domain.Entites;

public class Conversation
{
    public long Id { get; set; }
    public ConversationType Type { get; set; }
    public string? Name { get; set; }
    public string? GroupPicture { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<ConversationMember> Members { get; set; } = new List<ConversationMember>();
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}

public enum ConversationType
{
    Direct = 0,
    Group = 1
}
