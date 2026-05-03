using Azure.Core;
using collentra_be.DTO.Request;
using collentra_be.Interface;
using collentra_be.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("send-invite/{groupId}/{invitedByUserId}")]
        public async Task<IActionResult> SendInvitation(Guid groupId, Guid invitedByUserId, [FromBody] string targetEmailUser)
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

        //    // GET api/projects/{projectId}/invitations
        //    // List semua undangan (Owner / Admin)
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

        //    // GET api/projects/{projectId}/members
        //    // List member aktif (semua member bisa lihat)
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

        //    // DELETE api/projects/{projectId}/members/{targetUserId}
        //    // Kick member
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

        //    // PATCH api/projects/{projectId}/members/{targetUserId}/role
        //    // Update role (Owner only)
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

    //// Endpoint accept/decline — route berbeda, tidak perlu projectId
    //[ApiController]
    //[Route("api/invitations")]
    //[Authorize]
    //public class InvitationAcceptController(IInvitationService invitationService) : ControllerBase
    //{
    //    private Guid CurrentUserId =>
    //        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    //    // POST api/invitations/accept
    //    [HttpPost("accept")]
    //    public async Task<IActionResult> Accept([FromBody] AcceptInvitationRequest request)
    //    {
    //        var result = await invitationService.AcceptInvitationAsync(request.Token, CurrentUserId);
    //        return result.Success ? Ok(result) : BadRequest(result);
    //    }

    //    // POST api/invitations/decline
    //    [HttpPost("decline")]
    //    public async Task<IActionResult> Decline([FromBody] AcceptInvitationRequest request)
    //    {
    //        var result = await invitationService.DeclineInvitationAsync(request.Token, CurrentUserId);
    //        return result.Success ? Ok(result) : BadRequest(result);
    //    }
    //}

    //public record UpdateRoleRequest(string Role);

}
