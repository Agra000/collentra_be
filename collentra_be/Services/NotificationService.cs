using collentra_be.Data;
using collentra_be.DTO.Response;
using collentra_be.Interface;
using collentra_be.Model;
using Microsoft.EntityFrameworkCore;

namespace collentra_be.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<NotificationModel>> getAllActivityNotification(Guid targetId)
        {
            try
            {
                return await _context.Notifications
                    .Where(a => a.TargetId == targetId)
                    .Select(a => new NotificationModel
                    {
                        Id = a.Id,
                        GroupId = a.GroupId,
                        Title = a.Title,
                        Description = a.Description,
                        TargetId = a.TargetId,
                        isOpen = a.isOpen,
                        CreatedAt = a.CreatedAt
                    })
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<NotificationModel>();
            }
        }

        public async Task<ResultMessageResponse> markAllAsRead(Guid userId)
        {
            try
            {
                var getUserEmail = await _context.Users
                    .Where(x => x.user_id == userId && x.isActive)
                    .Select(e => e.email)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrEmpty(getUserEmail))
                {
                    throw new Exception();
                }

                var invitationRows = await _context.GroupInvitations
                    .Where(x => x.Email == getUserEmail
                            && x.Status == "Pending"
                            && x.ExpiresAt > DateTime.Now)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(n => n.isOpen, true)
                        .SetProperty(n => n.UpdatedAt, DateTime.UtcNow) 
                    );

                var activityRows = await _context.Notifications
                    .Where(x => x.TargetId == userId)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(n => n.isOpen, true)
                        .SetProperty(n => n.UpdatedAt, DateTime.UtcNow)
                    );

                return new ResultMessageResponse
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception ex) 
            {
                return new ResultMessageResponse
                {
                    Status = false,
                    Message = $"Server Error. Please Try Again ! {ex}"
                };
            }
        }

    }
}
