namespace collentra_be.DTO.Request
{
    public class LoginDTO
    {
        public string email { get; set; }
        public string password { get; set; }
        public string tokenCaptcha { get; set; }
    }
}
