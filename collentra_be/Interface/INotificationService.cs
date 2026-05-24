using collentra_be.DTO.Response;
using collentra_be.Model;

namespace collentra_be.Interface
{
    public interface INotificationService
    {
        Task<List<NotificationModel>> getAllActivityNotification(Guid targetId);
        Task<ResultMessageResponse> markAllAsRead(Guid userId);
    }
}
