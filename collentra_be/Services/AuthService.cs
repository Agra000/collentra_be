using BCrypt.Net;
using collentra_be.Data;
using collentra_be.Interface;
using collentra_be.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

                return "Login Successfully";
            }
            catch (Exception ex)
            {
                return $"{ex}";
            }
        }

    }
}
