using collentra_be.Data;
using collentra_be.DTO.Request;
using collentra_be.DTO.Response;
using collentra_be.Interface;
using collentra_be.Model;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace collentra_be.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public TaskService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        private async Task<GroupMemberModel?> checkAdminRole(Guid groupId, Guid userId)
        {
            return await _context.GroupMembers
                        .Where(x => x.GroupId == groupId
                        && x.UserId == userId
                        && x.Role == "Admin"
                        && !x.isLeaving)
                        .FirstOrDefaultAsync();
        }

        private async Task<UserModel?> isUserActive(Guid userId)
        {
            return await _context.Users
                .Where(x => x.user_id == userId
                && x.isActive)
                .FirstOrDefaultAsync();
        }

        private async Task<GroupMemberModel?> findUser(Guid groupId, Guid userId)
        {
            return await _context.GroupMembers
                    .Where(x => x.GroupId == groupId
                    && x.UserId == userId
                    && !x.isLeaving)
                    .FirstOrDefaultAsync();
        }

        public async Task<List<GetTaskDeadlineResponse>> getTaskDeadline(Guid asigneeId)
        {
            try
            {
                return await _context.Tasks
                    .Where(a => a.AssigneeId == asigneeId)
                    .Select(a => new GetTaskDeadlineResponse
                    {
                        Id = a.Id,
                        GroupName = a.Group.Name,
                        GroupId = a.GroupId,
                        Title = a.Title,
                        Description = a.Description,
                        AssigneeId = a.AssigneeId,
                        stats = a.Status,
                        Priority = a.Priority,
                        DueDate = a.DueDate,
                        CompletedAt = a.CompletedAt,
                    })
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<GetTaskDeadlineResponse>();
            }
        }

        public async Task<GetHomeResponse> getHomeInformation(Guid userId)
        {
            try
            {
                var groupCount = await _context.GroupMembers
                    .Where(x => x.UserId == userId
                    && !x.isLeaving)
                    .CountAsync();

                var taskCompleted = await _context.Tasks
                    .Where(x => x.AssigneeId == userId
                    && x.Status == "Done"
                    && !x.isDeleted)
                    .CountAsync();

                var taskRemaining = await _context.Tasks
                    .Where(x => x.AssigneeId == userId
                    && x.Status != "Done"
                    && !x.isDeleted)
                    .CountAsync();

                return new GetHomeResponse
                {
                    status = true,
                    groupCount = groupCount,
                    taskCompleted = taskCompleted,
                    taskRemaining = taskRemaining,
                    teamPerformance = 0
                };
            }
            catch (Exception ex) 
            {
                return new GetHomeResponse
                {
                    status = false,
                    message = $"Server Error. Please Try Again !"
                };
            }
        }

        public async Task<ResultMessageResponse> AddNewTask(Guid userId, TaskRequest req)
        {
            try
            {
                var isActive = await isUserActive(req.AssigneeId);
                if (isActive == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"This user are inactive !"
                    };
                }

                if (req.DueDate.Date <= DateTime.Now.Date)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"You can't add task on this deadline !"
                    };
                }

                var checkAdmin = await findUser(req.GroupId, userId);
                if (checkAdmin == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"You are not member on this group !"
                    };
                }

                var isAdmin = await checkAdminRole(req.GroupId, userId);
                if (isAdmin == null) 
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"You are not the owner on this group !"
                    };
                }

                var checkMember = await findUser(req.GroupId, req.AssigneeId);
                if (checkMember == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"This person is not member on this group !"
                    };
                }

                var checkTask = await _context.Tasks
                    .Where(x => x.GroupId == req.GroupId
                    && x.AssigneeId == req.AssigneeId
                    && x.Title == req.Title
                    && x.Status != "Done"
                    && !x.isDeleted)
                    .FirstOrDefaultAsync();

                if (checkTask != null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"You already gave this person same task !"
                    };
                }

                var task = new TaskModel
                {
                    GroupId = req.GroupId,
                    Title = req.Title,
                    Description = req.Description,
                    AssigneeId = req.AssigneeId,
                    Status = req.Status,
                    Priority = req.Priority,
                    DueDate = req.DueDate,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId
                };

                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();

                return new ResultMessageResponse
                {
                    Status = true,
                    Message = $"Successfully Adding Task !"
                };
            } 
            catch (Exception ex) 
            {
                return new ResultMessageResponse
                {
                    Status = false,
                    Message = $"Server Error. Please Try Again !"
                };
            }
        }

        public async Task<ResultMessageResponse> CompleteTask(TaskStatusRequest req)
        {
            try
            {
                var isActive = await isUserActive(req.leaderId);
                if (isActive == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"This user are inactive !"
                    };
                }

                var task = await _context.Tasks
                    .Where(x => x.Id == req.taskId
                    && x.Status != "Done"
                    && !x.isDeleted)
                    .FirstOrDefaultAsync();

                if(task == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"Task not Found ! !"
                    };
                }

                task.Status = "Done";
                task.CompletedAt = DateTime.Now;
                task.UpdatedBy = req.leaderId;
                task.UpdatedAt = DateTime.Now;
                
                await _context.SaveChangesAsync();

                return new ResultMessageResponse
                {
                    Status = true,
                    Message = $"Task Completed successfully !"
                };
            }
            catch (Exception ex)
            {
                return new ResultMessageResponse
                {
                    Status = false,
                    Message = $"Server Error. Please Try Again !"
                };
            }
        }
        public async Task<ResultMessageResponse> TerminateTask(TaskStatusRequest req)
        {
            try
            {
                var isActive = await isUserActive(req.leaderId);
                if (isActive == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"This user are inactive !"
                    };
                }

                var task = await _context.Tasks
                    .Where(x => x.Id == req.taskId
                    && x.Status != "Done"
                    && !x.isDeleted)
                    .FirstOrDefaultAsync();

                if (task == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"Task not Found ! !"
                    };
                }

                task.isDeleted = true;
                task.CompletedAt = DateTime.Now;
                task.UpdatedBy = req.leaderId;
                task.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new ResultMessageResponse
                {
                    Status = true,
                    Message = $"Task Terminated successfully !"
                };
            }
            catch (Exception ex)
            {
                return new ResultMessageResponse
                {
                    Status = false,
                    Message = $"Server Error. Please Try Again !"
                };
            }
        }

        public async Task<GetEditTasksResponse?> GetEditTask(Guid taskId)
        {
            try
            {
                return await _context.Tasks
                    .Where(x => x.Id == taskId && !x.isDeleted)
                    .Select(a => new GetEditTasksResponse
                    {
                        GroupId = a.GroupId,
                        Title = a.Title,
                        Description = a.Description,
                        AssigneeId = a.AssigneeId,
                        AssigneeName = a.Users.username,
                        Status = a.Status,
                        Priority = a.Priority,
                        DueDate = a.DueDate,
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return new GetEditTasksResponse();
            }
        }

        public async Task<ResultMessageResponse> EditTask(Guid userId, Guid taskId, TaskRequest req)
        {
            try
            {
                var isActive = await isUserActive(req.AssigneeId);
                if (isActive == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"This user are inactive !"
                    };
                }

                var checkMember = await findUser(req.GroupId, req.AssigneeId);
                if (checkMember == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"This person is not member on this group !"
                    };
                }
                
                var checkAdmin = await findUser(req.GroupId, userId);
                if (checkAdmin == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"You are not member on this group !"
                    };
                }

                var isAdmin = await checkAdminRole(req.GroupId, userId);
                if (isAdmin == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"You are not the owner on this group !"
                    };
                }

                var checkTask = await _context.Tasks
                    .Where(x => x.Id == taskId
                    && x.AssigneeId == req.AssigneeId
                    && x.Status != "Done"
                    && !x.isDeleted)
                    .FirstOrDefaultAsync();

                if (checkTask == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"Task not found !"
                    };
                }

                if (req.DueDate.Date <= DateTime.Now.Date)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"You can't update task on this deadline !"
                    };
                }

                checkTask.Title = req.Title;
                checkTask.Description = req.Description;
                checkTask.Status = req.Status;
                checkTask.Priority = req.Priority;
                checkTask.DueDate = req.DueDate;
                checkTask.UpdatedBy = userId;
                checkTask.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new ResultMessageResponse
                {
                    Status = true,
                    Message = $"Successfully Update Task !"
                };
            }
            catch (Exception ex)
            {
                return new ResultMessageResponse
                {
                    Status = false,
                    Message = $"Server Error. Please Try Again !"
                };
            }
        }

    }
}
