using collentra_be.DTO.Request;
using collentra_be.DTO.Response;
using collentra_be.Model;
using Microsoft.AspNetCore.Mvc;

namespace collentra_be.Interface
{
    public interface IRateCommentService
    {
        Task<List<PeopleDirectoryResponse>> GetAllUser(Guid userId);
        Task<UserModel> getUserById(Guid userId);
        Task<GetRatingResponse?> GetRating(Guid userId);
        Task<List<GetRatingResponse>?> GetRateAndComment(Guid userId);
        Task<ResultMessageResponse> RateAndCommentPerson(Guid userId, RateCommentPersonRequest req);
    }
}
