using Microsoft.AspNetCore.Mvc;
using tiketin_b.DTO;

namespace collentra_be.Interface
{
    public interface IAuthService
    {
        Task<bool> IsEmailUnique(string email);
        Task<bool> Register(RegistDTO request);
        Task<string> Login(LoginDTO request);
    }
}
