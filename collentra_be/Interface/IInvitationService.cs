using collentra_be.DTO.Request;
using collentra_be.DTO.Response;
using collentra_be.Model;

namespace collentra_be.Interface
{
    public interface IInvitationService
    {
        Task<List<GroupInvitationModel>> GetAllInvitation(Guid userId);
        Task<ResultMessageResponse> SendInvitationAsync(Guid projectId, Guid invitedByUserId, string targetEmailUser);
        Task<ResultMessageResponse> InviterTargetResponse(AcceptInvitiationRequest req);
        //Task<List<InvitationResponse>> GetProjectInvitationsAsync(Guid projectId, Guid requestingUserId);
    }
}
