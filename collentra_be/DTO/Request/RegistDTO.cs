namespace collentra_be.DTO.Request
{
    public class RegistDTO
    {
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string ConfirmPassword { get; set; }
        public char gender { get; set; }
        public string tokenCaptcha { get; set; }
        public DateTime dob { get; set; }
    }
}
