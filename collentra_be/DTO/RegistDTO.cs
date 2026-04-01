namespace tiketin_b.DTO
{
    public class RegistDTO
    {
        public string? username { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public string? ConfirmPassword { get; set; }
        public int gender { get; set; }
        public DateTime dob { get; set; }
    }
}
