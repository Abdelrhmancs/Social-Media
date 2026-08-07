using System;

namespace Application.DTOs.ChatDTOs
{
    public class ReadReceiptDto
    {
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string? UserPic { get; set; }
        public DateTime ReadAt { get; set; }
    }
}
