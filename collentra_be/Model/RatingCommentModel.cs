using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace collentra_be.Model
{
    public class RatingCommentModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public int Rate { get; set; }
        public string? Comment { get; set; }
        public Guid TargetId { get; set; }
        public bool IsDeleted { get; set; }

        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }


        [ForeignKey("CreatedBy")]
        public UserModel Rater { get; set; }

        [ForeignKey("TargetId")]
        public UserModel Users { get; set; }

        [ForeignKey("GroupId")]
        public GroupModel Group { get; set; }
    }
}
