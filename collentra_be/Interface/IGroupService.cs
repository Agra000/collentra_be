using collentra_be.DTO.Request;
using collentra_be.DTO.Response;
using collentra_be.Model;
using Microsoft.AspNetCore.Mvc;

namespace collentra_be.Interface
{
    public interface IGroupService
    {
        Task<List<GetAllGroupResponse>> GetAllGroup(Guid userId);
        Task<object?> GetGroupDetail(Guid groupId);
        Task<ResultMessageResponse> AddNewGroup(GroupRequest req);
        Task<ResultMessageResponse> RemoveGroup(Guid groupId, string userId);
        Task<ResultMessageResponse> UpdateGroup(Guid groupId, GroupRequest req);
    }
}
