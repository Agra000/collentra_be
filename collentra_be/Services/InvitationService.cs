using collentra_be.Data;
using collentra_be.DTO.Request;
using collentra_be.DTO.Response;
using collentra_be.Interface;
using collentra_be.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace collentra_be.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly ApplicationDbContext _context;

        public InvitationService(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<GroupMemberModel?> checkAdminRole(Guid groupId, Guid userId)
        {
            return await _context.GroupMembers
                        .Where(x => x.GroupId == groupId
                        && x.UserId == userId
                        && x.Role == "Admin"
                        && x.isLeaving == false)
                        .FirstOrDefaultAsync();
        }

        private async Task<UserModel?> getUserByEmail(string targetEmailUser)
        {
            return await _context.Users
                    .Where(x => x.email == targetEmailUser
                    && x.isActive)
                    .FirstOrDefaultAsync();
        }

        private async Task<GroupInvitationModel?> checkInvitationStatus(Guid groupId, string targetEmailUser)
        {
            return await _context.GroupInvitations
                        .Where(x => x.GroupId == groupId
                        && x.Email == targetEmailUser
                        && x.Status == "Pending"
                        && x.ExpiresAt > DateTime.Now)
                        .FirstOrDefaultAsync();
        }

        private async Task<GroupMemberModel?> checkExistingMember(Guid groupId, Guid userId)
        {
            return await _context.GroupMembers
                        .Where(x => x.GroupId == groupId
                        && x.UserId == userId
                        && x.isLeaving == false)
                        .FirstOrDefaultAsync();
        }

        private async Task<GroupModel?> getGroupById(Guid groupId, Guid userId)
        {
            return await _context.Groups
                        .Where(x => x.Id == groupId
                            && x.OwnerId == userId
                            && x.isDeleted == false)
                        .FirstOrDefaultAsync();
        }

        public async Task<List<SearchUserInviteResponse>> SearchUsers(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || email.Length < 2)
                    return new List<SearchUserInviteResponse>();

                return await _context.Users
                    .Where(u => u.email.StartsWith(email))
                    .Take(5)
                    .Select(u => new SearchUserInviteResponse
                    {
                        userId = u.user_id,
                        username = u.username,
                        rating = Math.Round(
                                _context.RatingComments
                                    .Where(r => r.TargetId == u.user_id && !r.IsDeleted)
                                    .Select(r => (double?)r.Rate)
                                    .Average() ?? 0.0,
                                1
                            ),
                        email = u.email
                    })
                    .ToListAsync();
            }
            catch (Exception ex) 
            {
                return new List<SearchUserInviteResponse>();
            }
        }

        public async Task<List<object>> GetAllInvitation(Guid userId)
        {
            try
            {
                var getUserEmail = await _context.Users
                    .Where(x => x.user_id == userId && x.isActive)
                    .Select(e => e.email)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrEmpty(getUserEmail))
                {
                    return new List<object>();
                }

                var invitations = await _context.GroupInvitations
                    .Where(x => x.Email == getUserEmail
                            && x.Status == "Pending"
                            && x.ExpiresAt > DateTime.Now)
                    .Select(x => new
                    {
                        id = x.Id,
                        type = "friend_request",
                        title = "Group Invitation",
                        message = $"You have been invited to join {x.Groups.Name}",
                        groupId = x.GroupId,
                        groupName = x.Groups.Name,
                        groupDescription = x.Groups.Description,
                        status = x.Status,
                        timestamp = x.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                        inviterName = x.Users.username,
                        inviterEmail = x.Users.email,
                        token = x.Token,
                        createdAt = x.CreatedAt,
                        x.ExpiresAt
                    })
                    .OrderByDescending(x => x.createdAt)
                    .ToListAsync();

                var result = invitations.Select(x =>
                {
                    TimeSpan expiredCountdown = x.ExpiresAt - DateTime.Now;

                    if (expiredCountdown < TimeSpan.Zero)
                    {
                        expiredCountdown = TimeSpan.Zero;
                    }

                    string countdownText;
                    if (expiredCountdown.TotalSeconds == 0)
                    {
                        countdownText = "Expired";
                    }
                    else if (expiredCountdown.Days >= 1)
                    {
                        countdownText = $"{expiredCountdown.Days} {(expiredCountdown.Days == 1 ? "day" : "days")} remaining";
                    }
                    else
                    {
                        countdownText = $"{expiredCountdown.ToString(@"hh\:mm\:ss")} remaining";
                    }

                    return new
                    {
                        id = x.id,
                        type = "friend_request",
                        title = "Group Invitation",
                        message = $"You have been invited to join {x.groupName}",
                        groupId = x.groupId,
                        groupName = x.groupName,
                        groupDescription = x.groupDescription,
                        status = x.status,
                        timestamp = x.createdAt.ToString("yyyy-MM-dd HH:mm"),
                        inviterName = x.inviterName,
                        inviterEmail = x.inviterEmail,
                        token = x.token,
                        createdAt = x.createdAt,
                        expiredCountdown = countdownText
                    };
                }).ToList();

                return result.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                return new List<object>();
            }
        }

        public async Task<ResultMessageResponse> SendInvitationAsync(Guid groupId, Guid invitedByUserId, string targetEmailUser)
        {
            try
            {
                var userTarget = await getUserByEmail(targetEmailUser);
                if (userTarget == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"{targetEmailUser} is not found !"
                    };
                }
                
                if (invitedByUserId == userTarget.user_id)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "You can't invite yourself !"
                    };
                }

                var group = await getGroupById(groupId, invitedByUserId);
                if (group == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "Group not found !"
                    };
                }

                var inviterMember = await checkAdminRole(group.Id, invitedByUserId);
                bool isOwner = group.OwnerId == invitedByUserId;
                bool isAdmin = inviterMember?.Role == "Admin";
                if (!isOwner && !isAdmin)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "Only group admin that can invite member !"
                    };
                }

                var existingMember = await checkExistingMember(groupId, userTarget.user_id);
                if (existingMember != null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"User with email: {targetEmailUser} is already on this project !"
                    };
                }

                var alreadyInvited = await checkInvitationStatus(groupId, targetEmailUser);
                if (alreadyInvited != null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "The invitation to this users is still active."
                    };
                }

                var token = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32))
                .Replace("+", "-").Replace("/", "_").Replace("=", "");

                var dataInvitation = new GroupInvitationModel
                {
                    GroupId = groupId,
                    InvitedByUserId = invitedByUserId,
                    Email = targetEmailUser,
                    Token = token,
                    Status = "Pending",
                    ExpiresAt = DateTime.Now.AddDays(7),
                    CreatedAt = DateTime.Now
                };

                _context.GroupInvitations.Add(dataInvitation);
                await _context.SaveChangesAsync();

                return new ResultMessageResponse
                {
                    Status = true,
                    Message = "Successfully invited!"
                };
            }
            catch (Exception ex) 
            {
                return new ResultMessageResponse
                {
                    Status = false,
                    Message = "Server Error. Please try again later !"
                };
            }
        }

        public async Task<ResultMessageResponse> InviterTargetResponse(AcceptInvitiationRequest req)
        {
            try
            {
                var userTarget = await getUserByEmail(req.currentEmail);
                if (userTarget == null) 
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"{req.currentEmail} is inactive or not found !"
                    };
                }

                var existingMember = await checkExistingMember(req.groupId, userTarget.user_id);
                if (existingMember != null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"User with email: {req.currentEmail} is already on this project !"
                    };
                }

                var invitationStillActive = await checkInvitationStatus(req.groupId, req.currentEmail);
                if (invitationStillActive == null) 
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "You dont have any invitation to this groups !"
                    };
                }

                if (invitationStillActive.Token != req.Token)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "You can't join this groups right now !"
                    };
                }

                var msg = "";
                if (req.Status)
                {
                    invitationStillActive.Status = "Accepted";
                    invitationStillActive.UpdatedAt = DateTime.Now;

                    var newMember = new GroupMemberModel
                    {
                        GroupId = invitationStillActive.GroupId,
                        UserId = userTarget.user_id,
                        Role = "Member",
                        isLeaving = false,
                        JoinedAt = DateTime.Now,
                    };

                    _context.GroupMembers.Add(newMember);
                    msg = "Successfully Joined Group !";
               
                    var notifToAdmin = new NotificationModel
                    { 
                        GroupId = invitationStillActive.GroupId,
                        Title = "New Member has arrived !",
                        Description = $"{userTarget.username} joined your group !",
                        TargetId = invitationStillActive.InvitedByUserId,
                        isOpen = false,
                        CreatedBy = userTarget.user_id,
                        CreatedAt = DateTime.Now
                    };

                    _context.Notifications.Add(notifToAdmin);
                }
                else
                {
                    invitationStillActive.Status = "Declined";
                    invitationStillActive.UpdatedAt = DateTime.Now;
                    msg = "Invitation rejected successfully !";
                }


                await _context.SaveChangesAsync();

                return new ResultMessageResponse
                {
                    Status = true,
                    Message = msg
                };
            }
            catch (Exception ex)
            {
                return new ResultMessageResponse
                {
                    Status = false,
                    Message = "Server Error. Please try again later !"
                };
            }
        }

        //public async Task<List<InvitationResponse>> GetProjectInvitationsAsync(Guid projectId, Guid requestingUserId)
        //{

        //}

    }
}
