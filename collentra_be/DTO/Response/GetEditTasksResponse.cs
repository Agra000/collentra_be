namespace collentra_be.DTO.Response
{
    public class GetEditTasksResponse
    {
        public Guid GroupId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid AssigneeId { get; set; }
        public string AssigneeName { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime DueDate { get; set; }
    }
}
