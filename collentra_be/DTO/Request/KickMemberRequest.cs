namespace collentra_be.DTO.Request
{
    public class KickMemberRequest
    {
        public Guid leaderId { get; set; }
        public string kickedMemberId { get; set; }
    }
}
