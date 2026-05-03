namespace collentra_be.DTO.Response
{
    public class InvitationResponse
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid InvitedByUserId { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Status { get; set; }
    }
}
