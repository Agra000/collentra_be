using Azure.Core;
using collentra_be.Interface;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using tiketin_b.DTO;

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
        public async Task<IActionResult> Register(RegistDTO r, [FromServices] IValidator<RegistDTO> validator)
        {
            try
            {
                var validationResult = await validator.ValidateAsync(r);

                if (!validationResult.IsValid)
                {
                    var firstError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed";

                    return BadRequest(new
                    {
                        status = false,
                        message = firstError
                    });
                }

                bool res = await _authService.Register(r);

                if (!res)
                {
                    return BadRequest(new
                    {
                        status = false,
                        message = "Registration Failed. Try Again"
                    });
                }

                return Ok(new
                {
                    status = true,
                    message = "Registration Successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = false,
                    message = "There are something wrong on databases",
                    error = ex.Message
                });
            }
        }

        [HttpPost("login")]
        public async Task<object> Login(LoginDTO r)
        {
            var res = await _authService.Login(r);

            if (res == "Email is not registered yet !!" || res == "Wrong Password !!")
            {
                return Unauthorized(new
                {
                    status = false,
                    message = res
                });
            } 
            else 
            {
                return new
                {
                    status = true,
                    token = res
                };
            }
        }
    }
}
