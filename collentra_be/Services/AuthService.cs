using Azure;
using Azure.Core;
using BCrypt.Net;
using collentra_be.Data;
using collentra_be.Interface;
using collentra_be.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using tiketin_b.DTO;

namespace collentra_be.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<bool> IsEmailUnique(string email) => !await _context.Users.AnyAsync(u => u.email == email);

        public async Task<bool> Register(RegistDTO r)
        {
            try
            {
                var data = new UserModel
                {
                    username = r.username,
                    email = r.email,
                    password = BCrypt.Net.BCrypt.HashPassword(r.password),
                    gender = r.gender,
                    dob = r.dob,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };

                _context.Users.Add(data);
                _context.SaveChanges();

                return true;
            } 
            catch 
            {
                return false;
            }
        }

        public async Task<string> Login(LoginDTO r) 
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.email == r.email);

                if (user == null)
                {
                    return "Email is not registered yet !!";
                }
                
                bool isPwValid = BCrypt.Net.BCrypt.Verify(r.password, user.password);

                if (!isPwValid)
                {
                    return "Wrong Password !!";
                } 

                var tokenString = GenerateJwtToken(user.email, user.username, user.Role);
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Path = "/",
                    Expires = DateTime.Now.AddDays(1)
                };
                //Response.Cookies.Append("token", tokenString, cookieOptions);

                return tokenString;
            }
            catch (Exception ex)
            {
                return $"{ex}";
            }
        }

        public string GenerateJwtToken(string email, string name, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email),
                new Claim("Role", role)
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

    }
}
