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
        public bool isLeaving { get; set; } = false;
        public DateTime JoinedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
