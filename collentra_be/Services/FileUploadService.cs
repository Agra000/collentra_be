using collentra_be.Data;
using collentra_be.DTO.Request;
using collentra_be.DTO.Response;
using collentra_be.Interface;
using collentra_be.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace collentra_be.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITaskService _taskService;

        public FileUploadService(ApplicationDbContext context, ITaskService taskService)
        {
            _context = context;
            _taskService = taskService;
        }

        public async Task<List<FileDownloadResponse>> GetAllDocs([FromQuery] Guid groupId)
        {
            try
            {
                return await _context.FileUpload
                    .Where(x => x.GroupId == groupId)
                    .Select(x => new FileDownloadResponse
                    {
                        Id = x.Id,
                        GroupId = x.GroupId,
                        SenderId = x.SenderId,
                        senderName = x.Sender.username,
                        FileName = x.FileName,
                        FileSize = x.FileSize,
                        FilePath = x.FilePath,
                        isDeleted = x.isDeleted,
                        CreatedAt = x.CreatedAt,
                    })
                    .OrderBy(x => x.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            { 
                return new List<FileDownloadResponse>();
            }
        }

        public async Task<ResultMessageResponse> uploadFile(UploadFileRequest req)
        {
            try
            {
                var task = await _context.Tasks
                    .Where(x => x.Id == req.TaskId
                    && x.GroupId == req.GroupId
                    && x.Status == "InProgress"
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

                var checkUser = await _context.Users
                    .Where(x => x.user_id == req.SenderId
                    && x.isActive)
                    .FirstOrDefaultAsync();

                if (checkUser == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"User is inActive right now ! !"
                    };
                }

                var checkMember = await _context.GroupMembers
                    .Where(x => x.GroupId == req.GroupId
                    && x.UserId == req.SenderId
                    && !x.isLeaving)
                    .FirstOrDefaultAsync();

                if (checkMember == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"You are not member on this group !"
                    };
                }

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(req.File.FileName)}";
                var targetFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                var fullPath = Path.Combine(targetFolder, uniqueFileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await req.File.CopyToAsync(stream);
                }

                double fileSizeKB = (double)req.File.Length / 1024;
                var downloadUrl = $"https://localhost:7283/shared-documents/{uniqueFileName}";
                
                var uploadDocument = new FileUploadModel {
                    GroupId = req.GroupId,
                    SenderId = req.SenderId,
                    FileName = req.File.FileName,
                    FileSize = $"{fileSizeKB:F2} kB",
                    FilePath = downloadUrl,
                    isDeleted = false,
                    CreatedBy = req.SenderId,
                    CreatedAt = DateTime.Now
                };

                var statusData = new TaskStatusRequest {
                    groupId = checkMember.GroupId,
                    leaderId = checkMember.UserId,
                    statusTask = "In Review",
                    taskId = task.Id
                };

                _context.FileUpload.Add(uploadDocument);
                await _context.SaveChangesAsync();

                var changeStatus = await _taskService.ChangeTaskStatus(statusData);
                if (!changeStatus.Status)
                {
                    return new ResultMessageResponse
                    {
                        Status = changeStatus.Status,
                        Message = changeStatus.Message
                    };
                }
               
                return new ResultMessageResponse
                {
                    Status = true,
                    Message = $"Successfully Upload File !!",
                    url = downloadUrl
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
