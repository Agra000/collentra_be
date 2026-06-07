using collentra_be.Data;
using collentra_be.DTO.Request;
using collentra_be.DTO.Response;
using collentra_be.Interface;
using collentra_be.Model;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace collentra_be.Services
{
    public class RateCommentService : IRateCommentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public RateCommentService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        private async Task<UserModel?> isUserActive(Guid userId)
        {
            return await _context.Users
                .Where(x => x.user_id == userId
                && x.isActive)
                .FirstOrDefaultAsync();
        }

        public async Task<List<PeopleDirectoryResponse>> GetAllUser(Guid userId)
        {
            return await _context.Users
                    .Where(x => x.isActive && x.user_id != userId)
                    .Select(x => new PeopleDirectoryResponse
                    {
                        id = x.user_id,
                        name = x.username,
                        emailMember = x.email,
                        groupsJoined = _context.GroupMembers
                                .Count(g => g.UserId == x.user_id && !g.isLeaving),

                        rating = Math.Round(
                                _context.RatingComments
                                    .Where(r => r.TargetId == x.user_id && !r.IsDeleted)
                                    .Select(r => (double?)r.Rate)
                                    .Average() ?? 0.0, 
                                1
                            )
                    })
                    .ToListAsync();
        }

        public async Task<UserModel?> getUserById(Guid userId)
        {
            try
            {
                var isActive = await isUserActive(userId);
                if (isActive == null)
                {
                    return new UserModel();
                }

                return isActive;
            }
            catch (Exception ex)
            {
                return new UserModel();
            }
        }

        public async Task<GetRatingResponse?> GetRating(Guid userId)
        {
            try
            {
                var isActive = await isUserActive(userId);
                if (isActive == null)
                {
                    return new GetRatingResponse();
                }

                return await _context.RatingComments
                    .Where(x => x.TargetId == userId && !x.IsDeleted)
                    .GroupBy(x => x.TargetId)
                    .Select(g => new GetRatingResponse
                    {
                        Rating = Math.Round(g.Average(x => x.Rate), 1),
                        RateCount = g.Count()
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return new GetRatingResponse();
            }
        }

        public async Task<List<GetRatingResponse>?> GetRateAndComment(Guid userId)
        {
            try
            {
                var isActive = await isUserActive(userId);
                if (isActive == null)
                {
                    return new List<GetRatingResponse>();
                }

                return await _context.RatingComments
                    .Where(x => x.TargetId == userId && !x.IsDeleted)
                    .Select(x => new GetRatingResponse
                    {
                        ratingId = x.Id,
                        Rating = x.Rate,
                        Comment = x.Comment,
                        groupName = x.Group.Name,
                        raterName = x.Rater.username,
                        raterEmail = x.Rater.email,
                        TimeRated = x.UpdatedAt == null ? x.CreatedAt : x.UpdatedAt
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<GetRatingResponse>();
            }
        }

        public async Task<ResultMessageResponse> RateAndCommentPerson(Guid userId, RateCommentPersonRequest req)
        {
            try
            {
                if(userId == req.TargetId)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"You Can't Rate yourself !"
                    };
                }

                var user = await isUserActive(req.TargetId);
                if (user == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"This user are inactive !"
                    };
                }

                var group = await _context.Groups
                    .Where(x => x.Id == req.GroupId && !x.isDeleted)
                    .FirstOrDefaultAsync();
                if(group == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "Group Not Found !"
                    };
                }

                var isMember1 = await _context.GroupMembers
                    .Where(x => x.GroupId == group.Id
                    && x.UserId == userId)
                    .FirstOrDefaultAsync();
                if (isMember1 == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"you are never join this group !"
                    };
                }

                var isMember = await _context.GroupMembers
                    .Where(x => x.GroupId == group.Id
                    && x.UserId == req.TargetId)
                    .FirstOrDefaultAsync();
                if (isMember == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = $"This user was never join same group with you !"
                    };
                }

                if (req.Rate < 0 || req.Rate > 5)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "You have to rate from 0 until 5 star !"
                    };
                }

                var checkRate = await _context.RatingComments
                    .Where(x => x.GroupId == group.Id
                    && x.TargetId == user.user_id
                    && x.CreatedBy == userId
                    && !x.IsDeleted)
                    .FirstOrDefaultAsync();
                
                if (checkRate == null)
                { 
                    var ratingAndComment = new RatingCommentModel 
                    { 
                        GroupId = group.Id,
                        Rate = req.Rate,
                        Comment = req.Comment,
                        TargetId = req.TargetId,
                        IsDeleted = false,
                        CreatedBy = userId,
                        CreatedAt = DateTime.Now
                    };

                    _context.RatingComments.Add(ratingAndComment);
                } 
                else
                {
                    checkRate.Rate = req.Rate;
                    checkRate.Comment = req.Comment;
                    checkRate.UpdatedBy = userId;
                    checkRate.UpdatedAt = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                return new ResultMessageResponse
                {
                    Status = true,
                    Message = "Rate and Comment Added Succesfully !"
                };
            }
            catch (Exception ex)
            {
                return new ResultMessageResponse
                {
                    Status = false,
                    Message = $"Server Error. Please Try Again {ex} !"
                };
            }
        }

    }
}
