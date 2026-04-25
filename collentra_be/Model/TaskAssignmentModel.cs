using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace collentra_be.Model
{
    public class TaskAssignmentModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public TaskModel Task { get; set; }
        public Guid UserId { get; set; }
        public UserModel User { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
