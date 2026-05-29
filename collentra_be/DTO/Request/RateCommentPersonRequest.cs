namespace collentra_be.DTO.Request
{
    public class RateCommentPersonRequest
    {
        public Guid GroupId { get; set; }
        public int Rate { get; set; }
        public string? Comment { get; set; }
        public Guid TargetId { get; set; }
    }
}
