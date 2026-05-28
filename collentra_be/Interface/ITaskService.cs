using collentra_be.DTO.Request;
using collentra_be.DTO.Response;
using collentra_be.Model;
using Microsoft.AspNetCore.Mvc;

namespace collentra_be.Interface
{
    public interface ITaskService
    {
        Task<List<GetTaskDeadlineResponse>> getTaskDeadline(Guid asigneeId);
        Task<GetHomeResponse> getHomeInformation(Guid userId);
        Task<ResultMessageResponse> AddNewTask(Guid userId, TaskRequest req);
        Task<GetEditTasksResponse?> GetEditTask(Guid taskId);
        Task<ResultMessageResponse> EditTask(Guid userId, Guid taskId, TaskRequest req);
    }
}
