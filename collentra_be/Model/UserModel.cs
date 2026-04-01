using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace collentra_be.Model
{
    public class UserModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid user_id { get; set; }
        public string? username { get; set; }
        public string? email { get; set; }
        public string? photo_profile { get; set; }
        public string? password { get; set; }
        public string Role { get; set; } = "User";
        public int gender { get; set; }
        public DateTime dob { get; set; }

        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
