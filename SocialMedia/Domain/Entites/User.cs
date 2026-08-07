using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace API.Domain.Entites;

public class User : IdentityUser
{
    public string Name { get; set; } = null!;
    public string? Bio { get; set; }
    public string? Pic { get; set; }
    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<Follow> FollowFolloweds { get; set; } = new List<Follow>();
    public virtual ICollection<Follow> FollowFollowers { get; set; } = new List<Follow>();
    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    public virtual ICollection<Notification> NotificationSenders { get; set; } = new List<Notification>();
    public virtual ICollection<Notification> NotificationUsers { get; set; } = new List<Notification>();
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<Share> Shares { get; set; } = new List<Share>();
    public virtual ICollection<ConversationMember> ConversationMembers { get; set; } = new List<ConversationMember>();
    public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public virtual ICollection<MessageReadReceipt> MessageReadReceipts { get; set; } = new List<MessageReadReceipt>();
}
