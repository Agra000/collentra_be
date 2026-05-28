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
        public Guid AssigneeId { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public decimal? EstimatedHours { get; set; }
        public decimal? ActualHours { get; set; }
        public int? SortOrder { get; set; }
        public bool isDeleted { get; set; }

        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }


        [ForeignKey("AssigneeId")]
        public UserModel Users { get; set; }

        [ForeignKey("GroupId")]
        public GroupModel Group { get; set; }
    }
}
