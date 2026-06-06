namespace collentra_be.DTO.Request
{
    public class TaskStatusRequest
    {
        public Guid groupId { get; set; }
        public Guid leaderId { get; set; }
        public Guid taskId { get; set; }
        public string? statusTask { get; set; }
    }
}
