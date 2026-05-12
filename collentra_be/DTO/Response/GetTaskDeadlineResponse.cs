namespace collentra_be.DTO.Response
{
    public class GetTaskDeadlineResponse
    {
        public Guid Id { get; set; }
        public string GroupName { get; set; }
        public Guid GroupId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public Guid AssigneeId { get; set; }
        public string stats { get; set; } // ganti status yang todo
        public string Priority { get; set; }

        public DateTime DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
