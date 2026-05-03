using collentra_be.DTO.Request;
using collentra_be.DTO.Response;
using collentra_be.Model;
using Microsoft.AspNetCore.Mvc;

namespace collentra_be.Interface
{
    public interface ITaskService
    {
        Task<ResultMessageResponse> AddNewTask(Guid userId, TaskRequest req);
    }
}
