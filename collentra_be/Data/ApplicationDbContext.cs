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
        public DbSet<GroupInvitationModel> GroupInvites { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<TaskAssignmentModel> TasksAssignments { get; set; }

    }
}
