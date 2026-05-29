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
                        Rating = x.Rate,
                        Comment = x.Comment,
                        RaterId = x.CreatedBy,
                        TimeRated = x.CreatedAt
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
