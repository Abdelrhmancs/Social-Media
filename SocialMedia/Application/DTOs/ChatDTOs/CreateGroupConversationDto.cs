using System.Collections.Generic;

namespace Application.DTOs.ChatDTOs
{
    public class CreateGroupConversationDto
    {
        public string Name { get; set; } = null!;
        public string? GroupPicture { get; set; }
        public List<string> MemberIds { get; set; } = new();
    }
}
