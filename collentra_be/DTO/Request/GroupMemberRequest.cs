using collentra_be.Model;

namespace collentra_be.DTO.Request
{
    public class GroupMemberRequest
    {
        public string GroupId { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; }
        public bool isLeaving { get; set; }
    }
}
