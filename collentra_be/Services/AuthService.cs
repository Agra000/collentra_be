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
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly IValidator<RegistDTO> _registValidator;

        public AuthService(ApplicationDbContext context, IConfiguration config, IValidator<RegistDTO> registValidator)
        {
            _context = context;
            _config = config;
            _registValidator = registValidator;
        }

        private string GenerateJwtToken(Guid id, string email, string name)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Sid, id.ToString()),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<ReCaptchaVerifiedResponse?> checkReCaptcha(string tokenCaptcha)
        {
            var secretKeyCaptcha = _config["ReCaptcha:SecretKey"];
            using var client = new HttpClient();
            var response = await client.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secretKeyCaptcha}&response={tokenCaptcha}", null
            );

            var jsonString = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ReCaptchaVerifiedResponse>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private async Task<UserModel?> isEmailRegistered(string email)
        {
            return await _context.Users
                    .Where(u => u.email == email
                        && u.isActive == true)
                    .FirstOrDefaultAsync();
        }

        public async Task<ResultMessageResponse> Register(RegistDTO r)
        {
            try
            {
                var googleResult = await checkReCaptcha(r.tokenCaptcha);
                if (googleResult == null || !googleResult.Success)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "Captcha is not valid or already expired !."
                    };
                }

                var user = await isEmailRegistered(r.email);
                if (user != null) 
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "Email already registered!"
                    };
                }

                var validationResult = await _registValidator.ValidateAsync(r);
                if (!validationResult.IsValid)
                {
                    var firstError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed";

                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = firstError
                    };
                }

                var data = new UserModel
                {
                    username = r.username,
                    email = r.email,
                    password = BCrypt.Net.BCrypt.HashPassword(r.password),
                    gender = r.gender,
                    dob = r.dob,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Users.Add(data);
                await _context.SaveChangesAsync();

                return new ResultMessageResponse
                {
                    Status = true,
                    Message = "Server Error. Please Try Again !"
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

        public async Task<ResultMessageResponse> Login(LoginDTO r) 
        {
            try
            {
                var googleResult = await checkReCaptcha(r.tokenCaptcha);
                if (googleResult == null || !googleResult.Success)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "Captcha is not valid or already expired !."
                    };
                }

                var user = await isEmailRegistered(r.email);
                if (user == null)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "Email is not registered yet !!"
                    };
                }

                bool isPwValid = BCrypt.Net.BCrypt.Verify(r.password, user.password);
                if (!isPwValid)
                {
                    return new ResultMessageResponse
                    {
                        Status = false,
                        Message = "Wrong Password !!"
                    };
                }

                var tokenString = GenerateJwtToken(user.user_id, user.email, user.username);
                //var cookieOptions = new CookieOptions
                //{
                //    HttpOnly = true,
                //    Secure = false,
                //    SameSite = SameSiteMode.Lax,
                //    Path = "/",
                //    Expires = DateTime.Now.AddDays(1)
                //};
                //Response.Cookies.Append("token", tokenString, cookieOptions);

                return new ResultMessageResponse
                {
                    Status = true,
                    Message = tokenString
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
