using collentra_be.DTO.Request;
using collentra_be.Interface;
using collentra_be.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace collentra_be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RatingCommentController : ControllerBase
    {
        private readonly IRateCommentService _IRateCommentService;

        public RatingCommentController(IRateCommentService rateCommentService)
        {
            _IRateCommentService = rateCommentService;
        }

        [HttpGet("get-all-user")]
        public async Task<IActionResult> GetAllUser([FromQuery] Guid userId)
        {
            var res = await _IRateCommentService.GetAllUser(userId);

            if (res == null || !res.Any())
            {
                return Ok(new { data = new List<object>(), message = "User not found" });
            }

            return Ok(new
            {
                data = res,
                message = "Success"
            });
        }

        [HttpGet("getUserById")]
        public async Task<IActionResult> getUserById([FromQuery] Guid userId)
        {
            var res = await _IRateCommentService.getUserById(userId);

            if (res == null)
            {
                return Ok(new { data = new List<object>(), message = "User not found!" });
            }

            return Ok(new
            {
                data = res,
                message = "Success"
            });
        }

        [HttpGet("get-rating")]
        public async Task<IActionResult> GetRating([FromQuery] Guid userId)
        {
            var res = await _IRateCommentService.GetRating(userId);

            if (res == null)
            {
                return Ok(new { data = new List<object>(), message = "You dont have any rating" });
            }

            return Ok(new
            {
                data = res,
                message = "Success"
            });
        }

        [HttpGet("get-rate-comment")]
        public async Task<IActionResult> GetRateAndComment([FromQuery] Guid userId)
        {
            var res = await _IRateCommentService.GetRateAndComment(userId);

            if (res == null || !res.Any())
            {
                return Ok(new { data = new List<object>(), message = "You dont have any rating" });
            }

            return Ok(new
            {
                data = res,
                message = "Success"
            });
        }

        [HttpPost("rate-person")]
        public async Task<IActionResult> RateAndCommentPerson([FromQuery] Guid userId, [FromBody] RateCommentPersonRequest req)
        {
            var res = await _IRateCommentService.RateAndCommentPerson(userId, req);

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


        [HttpPost("update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest req)
        {
            var res = await _IRateCommentService.UpdateUser(req);

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
