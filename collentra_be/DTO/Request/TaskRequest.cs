using collentra_be.Model;

namespace collentra_be.DTO.Request
{
    public class TaskRequest
    {
        public Guid GroupId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid AssigneeId { get; set; }
        public string? Status { get; set; }
        public string Priority { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public decimal? EstimatedHours { get; set; }
        public decimal? ActualHours { get; set; }
        public int SortOrder { get; set; }
    }
}
