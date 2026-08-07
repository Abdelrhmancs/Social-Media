using API.Domain.Entites;
using System;
using System.Collections.Generic;

namespace Application.DTOs.ChatDTOs
{
    public class ConversationDto
    {
        public long Id { get; set; }
        public ConversationType Type { get; set; }
        public string? Name { get; set; }
        public string? GroupPicture { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ConversationMemberDto> Members { get; set; } = new();
        public MessageDto? LastMessage { get; set; }
    }

    public class ConversationMemberDto
    {
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string? UserPic { get; set; }
        public MemberRole Role { get; set; }
        public DateTime JoinedAt { get; set; }
        public bool IsOnline { get; set; }
    }
}
