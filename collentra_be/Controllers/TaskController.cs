using collentra_be.DTO.Request;
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
