using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace collentra_be.Model
{
    public class UserModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid user_id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string? photo_profile { get; set; }
        public bool isEmailVerified { get; set; } = false;
        public bool isActive { get; set; } = true;
        public string password { get; set; }
        public char gender { get; set; }
        public DateTime dob { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
