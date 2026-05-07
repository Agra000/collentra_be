namespace collentra_be.DTO.Request
{
    public class AcceptInvitiationRequest
    {
        public Guid groupId { get; set; }
        public string currentEmail { get; set; }
        public string? Token { get; set; }
        public bool Status { get; set; }
    }
}
