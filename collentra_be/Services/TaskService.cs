using collentra_be.Data;
using collentra_be.DTO.Request;
using collentra_be.DTO.Response;
using collentra_be.Interface;
using collentra_be.Model;
using Microsoft.EntityFrameworkCore;

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
                    && x.Status == "Done")
                    .CountAsync();

                var taskRemaining = await _context.Tasks
                    .Where(x => x.AssigneeId == userId
                    && x.Status != "Done")
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
                if (req.DueDate.Date <= DateTime.Now.Date)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"You can't add task on this deadline !"
                    };
                }

                var ChekcOwner = await _context.Groups
                    .Where(x => x.Id == req.GroupId
                    && x.OwnerId == userId)
                    .FirstOrDefaultAsync();

                if (ChekcOwner == null) 
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"You are not the owner on this group !"
                    };
                }

                var checkUser = await _context.GroupMembers
                    .Where(x => x.GroupId == req.GroupId
                    && x.UserId == req.AssigneeId)
                    .FirstOrDefaultAsync();

                if (checkUser == null)
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
                    && x.Title == req.Title)
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
                    CreatedAt = DateTime.UtcNow,
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

    }
}
