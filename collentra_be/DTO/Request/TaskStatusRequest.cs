namespace collentra_be.DTO.Request
{
    public class TaskStatusRequest
    {
        public Guid leaderId { get; set; }
        public Guid taskId { get; set; }
    }
}
