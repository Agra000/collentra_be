using Azure.Core;
using collentra_be.DTO.Request;
using collentra_be.Interface;
using collentra_be.Model;
using collentra_be.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace collentra_be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvitationController : ControllerBase
    {
        private readonly IInvitationService _inviteService;

        public InvitationController(IInvitationService inviteService)
        {
            _inviteService = inviteService;
        }

        //private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetAllInvitation([FromQuery] Guid userId)
        {
            var res = await _inviteService.GetAllInvitation(userId);

            if (res == null || !res.Any())
            {
                return Ok(new { data = new List<object>(), message = "No invitation found" });
            }

            return Ok(new
            {
                data = res,
                message = "Success fetch invitation"
            });
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string email)
        {
            var res = await _inviteService.SearchUsers(email);

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

        [HttpPost("send-invite/{groupId}")]
        public async Task<IActionResult> SendInvitation(Guid groupId, [FromQuery] Guid invitedByUserId, [FromBody] string targetEmailUser)
        {
            var res = await _inviteService.SendInvitationAsync(groupId, invitedByUserId, targetEmailUser);

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

        [HttpPost("inviter-target-response")]
        public async Task<IActionResult> InviterTargetResponse([FromBody] AcceptInvitiationRequest req)
        {
            var res = await _inviteService.InviterTargetResponse(req);

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

        //    [HttpGet]
        //    public async Task<IActionResult> GetInvitations(Guid projectId)
        //    {
        //        try
        //        {
        //            var result = await _inviteService.GetProjectInvitationsAsync(projectId, CurrentUserId);
        //            return Ok(result);
        //        }
        //        catch (UnauthorizedAccessException) { return Forbid(); }
        //        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        //    }

        //    [HttpGet("/api/projects/{projectId}/members")]
        //    public async Task<IActionResult> GetMembers(Guid projectId)
        //    {
        //        try
        //        {
        //            var result = await _inviteService.GetProjectMembersAsync(projectId, CurrentUserId);
        //            return Ok(result);
        //        }
        //        catch (UnauthorizedAccessException) { return Forbid(); }
        //        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        //    }

        //    [HttpDelete("/api/projects/{projectId}/members/{targetUserId:guid}")]
        //    public async Task<IActionResult> RemoveMember(Guid projectId, Guid targetUserId)
        //    {
        //        try
        //        {
        //            var result = await _inviteService.RemoveMemberAsync(projectId, targetUserId, CurrentUserId);
        //            return result.Success ? Ok(result) : BadRequest(result);
        //        }
        //        catch (UnauthorizedAccessException) { return Forbid(); }
        //        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        //    }

        //    [HttpPatch("/api/projects/{projectId}/members/{targetUserId:guid}/role")]
        //    public async Task<IActionResult> UpdateRole(
        //        Guid projectId, Guid targetUserId, [FromBody] UpdateRoleRequest request)
        //    {
        //        try
        //        {
        //            var result = await _inviteService.UpdateMemberRoleAsync(
        //                projectId, targetUserId, request.Role, CurrentUserId);
        //            return result.Success ? Ok(result) : BadRequest(result);
        //        }
        //        catch (UnauthorizedAccessException) { return Forbid(); }
        //        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        //    }
    }
}
