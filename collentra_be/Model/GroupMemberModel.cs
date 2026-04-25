using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace collentra_be.Model
{
    public class GroupMemberModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public GroupModel Group { get; set; }
        public Guid UserId { get; set; }
        public UserModel User { get; set; }
        public string Role { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
