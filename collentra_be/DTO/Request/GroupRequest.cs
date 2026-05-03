namespace collentra_be.DTO.Request
{
    public class GroupRequest
    {
        public string Name { get; set; }
        public string userId { get; set; }
        public string Description { get; set; }
        public bool IsArchived { get; set; }
    }
}
