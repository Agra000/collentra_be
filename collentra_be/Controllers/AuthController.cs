using collentra_be.DTO.Request;
using collentra_be.Interface;
using Microsoft.AspNetCore.Mvc;

namespace collentra_be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) 
        { 
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistDTO r)
        {
            var res = await _authService.Register(r);

            if (!res.Status)
            {
                return BadRequest(new
                {
                    status = res.Status,
                    message = res.Message
                });
            }

            return Ok(new
            {
                status = res.Status,
                message = res.Message
            });
        }

        [HttpPost("login")]
        public async Task<object> Login(LoginDTO r)
        {
            var res = await _authService.Login(r);

            if (!res.Status)
            {
                return Unauthorized(new
                {
                    status = res.Status,
                    message = res.Message
                });
            } 
            else 
            {
                return new
                {
                    status = res.Status,
                    token = res.Message
                };
            }
        }
    }
}
