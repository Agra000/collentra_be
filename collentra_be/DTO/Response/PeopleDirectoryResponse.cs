namespace collentra_be.DTO.Response
{
    public class PeopleDirectoryResponse
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string emailMember { get; set; }
        public int groupsJoined { get; set; }
        public double rating { get; set; }
    }
}
