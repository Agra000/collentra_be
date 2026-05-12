using collentra_be.DTO.Request;
using collentra_be.DTO.Response;

namespace collentra_be.Interface
{
    public interface ITaskService
    {
        Task<List<GetTaskDeadlineResponse>> getTaskDeadline(Guid asigneeId);
        Task<GetHomeResponse> getHomeInformation(Guid userId);
        Task<ResultMessageResponse> AddNewTask(Guid userId, TaskRequest req);
    }
}
