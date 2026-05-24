using collentra_be.DTO.Request;
using collentra_be.Interface;
using collentra_be.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace collentra_be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notifService;

        public NotificationController(INotificationService notifService)
        {
            _notifService = notifService;
        }

        [HttpGet("getAllNotif")]
        public async Task<IActionResult> getAllActivityNotification([FromQuery] Guid targetId)
        {
            var res = await _notifService.getAllActivityNotification(targetId);

            if (res == null || !res.Any())
            {
                return Ok(new { data = new List<object>(), message = "Notifications not found" });
            }

            return Ok(new
            {
                data = res,
                message = "Success"
            });
        }

        [HttpPut("markAllRead")]
        public async Task<IActionResult> markAllAsRead([FromBody] Guid userId)
        {
            var res = await _notifService.markAllAsRead(userId);

            if (res.Status == false)
            {
                return BadRequest(new { data = new List<object>(), message = "ServerError" });
            }

            return Ok(new
            {
                data = res,
                message = "Success"
            });
        }

    }
}
