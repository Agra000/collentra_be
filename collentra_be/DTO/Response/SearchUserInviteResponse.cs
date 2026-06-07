namespace collentra_be.DTO.Response
{
    public class SearchUserInviteResponse
    {
        public Guid userId { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public double rating { get; set; }
    }
}
