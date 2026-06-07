namespace collentra_be.DTO.Request
{
    public class UpdateUserRequest
    {
        public Guid userId { get; set; }
        public string? bio { get; set; }
        public int? showComment { get; set; }
    }
}
