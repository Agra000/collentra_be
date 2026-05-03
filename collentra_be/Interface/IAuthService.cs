using collentra_be.DTO.Request;
using collentra_be.DTO.Response;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace collentra_be.Interface
{
    public interface IAuthService
    {
        Task<ResultMessageResponse> Register(RegistDTO r);
        Task<ResultMessageResponse> Login(LoginDTO r);
    }
}
