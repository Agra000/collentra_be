namespace collentra_be.DTO.Request
{
    public class UploadFileRequest
    {
        public Guid GroupId { get; set; }
        public Guid SenderId { get; set; }
        public IFormFile File { get; set; }
    }
}
