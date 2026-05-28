using collentra_be.Data;
using collentra_be.DTO.Request;
using collentra_be.DTO.Response;
using collentra_be.Interface;
using collentra_be.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;

namespace collentra_be.Services
{
    public class GroupService : IGroupService
    {
        private readonly ApplicationDbContext _context;

        public GroupService(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<GroupModel?> getGroupById(Guid groupId)
        {
            return await _context.Groups
                    .Where(x => x.Id == groupId
                        && x.isDeleted == false)
                    .FirstOrDefaultAsync();
        }

        private async Task<UserModel?> checkUserId(string userId)
        {
            Guid userIdGuid = Guid.Parse(userId);

            return await _context.Users
                    .Where(u => u.user_id == userIdGuid
                        && u.isActive == true)
                    .FirstOrDefaultAsync();
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

        private async Task<bool> addMember(Guid groupId, Guid userId)
        {
            var newGroup = await _context.Groups
                        .Where(x => x.Id == groupId
                            && x.isDeleted == false)
                        .FirstOrDefaultAsync();

            if (newGroup == null)
            {
                return false;
            }

            var member = new GroupMemberModel
            {
                GroupId = newGroup.Id,
                UserId = userId,
                Role = "Admin",
                isLeaving = false,
                JoinedAt = DateTime.Now,
            };

            _context.GroupMembers.Add(member);
            await _context.SaveChangesAsync();
            
            return true;
        }

        public async Task<List<GetAllGroupResponse>> GetAllGroup(Guid userId )
        {
            try
            {
                return await _context.GroupMembers
                    .Where(gm => gm.UserId == userId && !gm.isLeaving)
                    .Select(gm => new GetAllGroupResponse
                    {
                        groupId = gm.Group.Id,
                        groupName = gm.Group.Name,
                        Description = gm.Group.Description ?? "",
                        LeaderName = _context.Users
                            .Where(u => u.user_id == gm.Group.OwnerId)
                            .Select(u => u.username)
                            .FirstOrDefault() ?? "No Owner",

                        MemberCount = _context.GroupMembers.Count(x => x.GroupId == gm.GroupId && !x.isLeaving),
                        taskTotal = _context.Tasks.Count(x => x.GroupId == gm.GroupId
                                    && !x.isDeleted),
                        taskComplete = _context.Tasks.Count(x => x.GroupId == gm.GroupId 
                                    && x.Status == "Done" 
                                    && !x.isDeleted)
                    })
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<GetAllGroupResponse>();
            }
        }

        public async Task<object?> GetGroupDetail(Guid groupId)
        {
            var groupDetail = await _context.Groups
                .Where(g => g.Id == groupId && !g.isDeleted)
                .Select(g => new
                {
                    groupId = g.Id,
                    groupName = g.Name,
                    description = g.Description,
                    groupOwnerId = g.OwnerId,
                    isArchived = g.IsArchived,

                    members = _context.GroupMembers
                        .Where(gm => !gm.isLeaving && gm.GroupId == groupId)
                        .Select(gm => new
                        {
                            id = gm.UserId,
                            name = gm.User.username,
                            emailMember = gm.User.email,
                            role = gm.Role,
                            tasksCompleted = _context.Tasks.Count(t => t.GroupId == groupId 
                                            && t.AssigneeId == gm.UserId 
                                            && t.Status == "Done" 
                                            && !t.isDeleted),
                            totalTasks = _context.Tasks.Count(t => t.GroupId == groupId
                                            && t.AssigneeId == gm.UserId
                                            && !t.isDeleted)
                        }).ToList(),

                    tasks = _context.Tasks
                        .Where(t => t.GroupId == groupId)
                        .Select(t => new
                        {
                            id = t.Id,
                            name = t.Title,
                            assignee = t.Users.username,
                            assigneeId = t.Users.user_id,
                            status = t.isDeleted ? "Terminate" : t.Status,
                            priority = t.Priority,
                            description = t.Description,
                            dueDate = t.DueDate.ToString("yyyy-MM-dd")
                        })
                        .OrderBy(t => t.status == "Done" ? 5 :
                            t.status == "Terminate" ? 6 :
                            t.priority == "Critical" ? 1 :
                            t.priority == "High" ? 2 :
                            t.priority == "Medium" ? 3 :
                            t.priority == "Low" ? 4 : 7)
                        .ToList(),

                    taskTotal = _context.Tasks.Count(t => t.GroupId == groupId 
                        && !t.isDeleted),
                    taskComplete = _context.Tasks.Count(t => t.GroupId == groupId 
                        && t.Status == "Done" 
                        && !t.isDeleted),
                    status = true
                })
                .FirstOrDefaultAsync();

            if (groupDetail == null) return null; 

            return groupDetail;
        }

        public async Task<ResultMessageResponse> AddNewGroup(GroupRequest req)
        {
            try
            {
                var user = await checkUserId(req.userId);
                if (user == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "User Not Found !!"
                    };
                }

                DateTime created_at = DateTime.Now;
                var data = new GroupModel
                {
                    Name = req.Name,
                    Description = req.Description,
                    OwnerId = user.user_id,
                    IsArchived = false,
                    CreatedBy = user.user_id,
                    CreatedAt = created_at,
                };
                
                _context.Groups.Add(data);
                await _context.SaveChangesAsync();

                var newGroupId = await _context.Groups
                    .Where(x => x.CreatedAt == created_at
                        && x.OwnerId == user.user_id
                        && x.isDeleted == false)
                    .FirstOrDefaultAsync();

                if (newGroupId == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "Group Id Not Found !!"
                    };
                }

                bool addMemberRes = await addMember(newGroupId.Id, user.user_id);
                if (!addMemberRes)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "Add Member Failed !!"
                    };
                }

                return new ResultMessageResponse
                {
                    Status = true,
                    Message = "Group Added Succesfully !"
                };
            }
            catch (Exception ex)
            {
                return new ResultMessageResponse
                {
                    Status = false,
                    Message = $"Server Error. Please Try Again ! {ex}"
                };
            }
        }

        public async Task<ResultMessageResponse> KickMember(Guid groupId, KickMemberRequest req)
        {
            try
            {
                var group = await getGroupById(groupId);
                if (group == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "Group Not Found !!"
                    };
                }

                var user = await checkUserId(req.kickedMemberId);
                if (user == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "User Not Found !!"
                    };
                }

                if (user.user_id == req.leaderId)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "You can't kick yourself !!"
                    };
                }

                var isAdmin = await checkAdminRole(groupId, req.leaderId);
                if (isAdmin == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "You are not admin on this group !!"
                    };
                }

                var isMemberInGroup = await _context.GroupMembers
                                    .Where(x => x.GroupId == groupId
                                    && x.UserId == user.user_id
                                    && x.Role == "Member"
                                    && x.isLeaving == false)
                                    .FirstOrDefaultAsync();

                if (isMemberInGroup == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "This person are not member on this group !!"
                    };
                }

                isMemberInGroup.isLeaving = true;
                isMemberInGroup.UpdatedBy = req.leaderId;
                isMemberInGroup.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new ResultMessageResponse
                {
                    Status = true,
                    Message = $"{user.email} kicked Successfully!"
                };
            }
            catch (Exception ex)
            {
                return new ResultMessageResponse
                {
                    Status = false,
                    Message = $"Server Error. Please Try Again ! {ex}"
                };
            }
        }

        public async Task<ResultMessageResponse> RemoveGroup(Guid groupId, string userId)
        {
            try
            {
                var deletedGroup = await getGroupById(groupId);
                if (deletedGroup == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "Group Not Found !!"
                    };
                }

                var user = await checkUserId(userId);
                if (user == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "User Not Found !!"
                    };
                }

                var isAdmin = await checkAdminRole(groupId, user.user_id);
                if (isAdmin == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "You are not admin on this group !!"
                    };
                }

                deletedGroup.isDeleted = true;
                deletedGroup.UpdatedBy = user.user_id; 
                deletedGroup.IsArchived = false;
                deletedGroup.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new ResultMessageResponse
                {
                    Status = true,
                    Message = "Group Deleted Successfully!"
                };
            }
            catch (Exception ex)
            {
                return new ResultMessageResponse
                {
                    Status = false,
                    Message = $"Server Error. Please Try Again ! {ex}"
                };
            }
        }

        public async Task<ResultMessageResponse> UpdateGroup(Guid groupId, GroupRequest req)
        {
            try
            {
                var updatedGroup = await getGroupById(groupId);
                if (updatedGroup == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "Group Not Found !!"
                    };
                }
                
                var user = await checkUserId(req.userId); 
                if (user == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "User Not Found !!"
                    };
                }

                var isAdmin = await checkAdminRole(groupId, user.user_id);
                if (isAdmin == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "You are not admin on this group !!"
                    };
                }

                updatedGroup.Name = req.Name;
                updatedGroup.Description = req.Description;
                updatedGroup.IsArchived = req.IsArchived;
                updatedGroup.UpdatedBy = user.user_id;
                updatedGroup.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new ResultMessageResponse
                {
                    Status = true,
                    Message = "Group Updated Successfully!"
                };
            }
            catch (Exception ex)
            {
                return new ResultMessageResponse
                {
                    Status = false,
                    Message = $"Server Error. Please Try Again ! {ex}"
                };
            }
        }

    }
}
