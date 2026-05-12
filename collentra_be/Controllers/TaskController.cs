using collentra_be.DTO.Request;
using collentra_be.DTO.Response;
using collentra_be.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace collentra_be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet("getTaskDeadline")]
        public async Task<IActionResult> getTaskDeadline([FromQuery] Guid assigneeId)
        {
            var res = await _taskService.getTaskDeadline(assigneeId);

            if (res == null)
            {
                return BadRequest(new { data = new List<object>(), message = "Tasks not found" });
            }

            return Ok(new
            {
                data = res,
                message = "Success"
            });
        }

        [HttpGet("getHomeInformation")]
        public async Task<GetHomeResponse> getHomeInformation([FromQuery] Guid userId)
        {
            var res = await _taskService.getHomeInformation(userId);

            if (!res.status)
            {
                return new GetHomeResponse
                {
                    status = res.status,
                    message = res.message
                };
            }
            else
            {
                return new GetHomeResponse
                {
                    status = res.status,
                    groupCount = res.groupCount,
                    taskRemaining = res.taskRemaining,
                    taskCompleted = res.taskCompleted,
                    teamPerformance = res.teamPerformance
                };
            }
        }

        [HttpPost("create-task")]
        public async Task<IActionResult> AddNewTask([FromQuery] Guid userId, [FromBody] TaskRequest req)
        {
            var res = await _taskService.AddNewTask(userId, req);

            if (!res.Status)
            {
                return BadRequest(new
                {
                    status = res.Status,
                    message = res.Message
                });
            }
            else
            {
                return Ok(new
                {
                    status = res.Status,
                    message = res.Message
                });
            }
        }
    }
}
