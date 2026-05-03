namespace collentra_be.DTO.Response
{
    public class GroupMemberResponse
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
