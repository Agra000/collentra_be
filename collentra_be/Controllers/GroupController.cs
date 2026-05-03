using Azure.Core;
using collentra_be.DTO.Request;
using collentra_be.Interface;
using collentra_be.Model;
using collentra_be.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace collentra_be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService) 
        {
            _groupService = groupService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGroup([FromQuery] Guid userId)
        {
            var res = await _groupService.GetAllGroup(userId);

            if (res == null || !res.Any())
            {
                return Ok(new { data = new List<object>(), message = "Groups not found" });
            }

            return Ok(new
            {
                data = res,
                message = "Success"
            });
        }

        [HttpGet("group-detail")]
        public async Task<IActionResult> GetGroupDetail([FromQuery] Guid groupId)
        {
            var res = await _groupService.GetGroupDetail(groupId);

            if (res == null)
            {
                return Ok(new { data = new List<object>(), message = "Groups not found" });
            }

            return Ok(new
            {
                data = res,
                message = "Success"
            });
        }

        [HttpPost("create-group")]
        public async Task<IActionResult> AddNewGroup(GroupRequest req)
        {
            var res = await _groupService.AddNewGroup(req);

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

        [HttpDelete("{groupId}/{userId}")]
        public async Task<IActionResult> RemoveGroup(Guid groupId, string userId)
        {
            var res = await _groupService.RemoveGroup(groupId, userId);

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

        [HttpPut("{groupId}")]
        public async Task<IActionResult> UpdateGroup(Guid groupId, [FromBody] GroupRequest req)
        {
            var res = await _groupService.UpdateGroup(groupId, req);

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
