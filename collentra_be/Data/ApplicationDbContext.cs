using collentra_be.Model;
using Microsoft.EntityFrameworkCore;

namespace collentra_be.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<GroupModel> Groups { get; set; }
        public DbSet<GroupMemberModel> GroupMembers { get; set; }
        public DbSet<GroupInvitationModel> GroupInvitations { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<NotificationModel> Notifications { get; set; }
        public DbSet<RatingCommentModel> RatingComments { get; set; }
        public DbSet<FileUploadModel> FileUpload { get; set; }
    }
}
