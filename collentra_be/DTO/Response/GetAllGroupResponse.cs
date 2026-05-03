namespace collentra_be.DTO.Response
{
    public class GetAllGroupResponse
    {
        public Guid groupId { get; set; }
        public string groupName { get; set; }
        public string Description { get; set; }
        public string LeaderName { get; set; }
        public int MemberCount { get; set; }
        public int taskTotal { get; set; }
        public int taskComplete { get; set; }
    }
}
