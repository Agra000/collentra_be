using collentra_be.DTO.Request;
using collentra_be.DTO.Response;
using collentra_be.Model;
using Microsoft.AspNetCore.Mvc;

namespace collentra_be.Interface
{
    public interface IFileUploadService
    {
        Task<List<FileDownloadResponse>> GetAllDocs([FromQuery] Guid groupId);
        Task<ResultMessageResponse> uploadFile([FromBody] UploadFileRequest req);
    }
}
