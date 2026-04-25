using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace collentra_be.Model
{
    public class TaskModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ProgressPercent { get; set; }
        public string Status { get; set; }
        public ICollection<TaskAssignmentModel> Assignments { get; set; }

        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
