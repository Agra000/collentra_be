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
    public class FileUploadController : ControllerBase
    {
        private readonly IFileUploadService _IFileUploadService;

        public FileUploadController(IFileUploadService fileUploadService)
        {
            _IFileUploadService = fileUploadService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDocs([FromQuery] Guid groupId)
        {
            var res = await _IFileUploadService.GetAllDocs(groupId);

            if (res == null || !res.Any())
            {
                return Ok(new { data = new List<object>(), message = "Documents not found" });
            }

            return Ok(new
            {
                data = res,
                message = "Success"
            });
        }

        [HttpPost("upload-file")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> uploadFile([FromForm] UploadFileRequest req)
        {
            if (req.File == null || req.File.Length == 0)
            {
                return BadRequest("Cannot Read File !");
            }

            var res = await _IFileUploadService.uploadFile(req);

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
