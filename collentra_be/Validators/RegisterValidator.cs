using collentra_be.Interface;
using FluentValidation;
using tiketin_b.DTO;

namespace tiketin_b.Validators
{
    public class RegisterValidator : AbstractValidator<RegistDTO>
    {
        public RegisterValidator(IAuthService authService) 
        {
            RuleFor(x => x.username)
                .NotEmpty().WithMessage("Username must be filled")
                .Matches(@"^\S+$").WithMessage("Username must be one word")
                .MaximumLength(10).WithMessage("Username at must be 10 maximum letter");

            RuleFor(x => x.email)
                .NotEmpty().WithMessage("Email must be filled")
                .MustAsync(async (e, _) => await authService.IsEmailUnique(e)).WithMessage("Email has been Registed");

            RuleFor(x => x.password)
                .NotEmpty().WithMessage("Password must be filled")
                // kali aje di pake
                //.Matches(@"[A-Z]").WithMessage("Password harus ada huruf besar")
                //.Matches(@"[0-9]").WithMessage("Password harus ada angka")
                //.Matches(@"[\!\?\*\.]").WithMessage("Password harus ada simbol (!?*.)")
                .MinimumLength(8).WithMessage("Password at least have 8 minimum letter");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm Password must be filled")
                .Equal(x => x.password).WithMessage("Confirm password is not same like password");

            RuleFor(x => x.gender)
                .NotEmpty().WithMessage("Gender must be filled");

            RuleFor(x => x.dob)
                .NotEmpty().WithMessage("Date of Birth must be filled")
                .LessThan(DateTime.Now.Date).WithMessage("Date of Birth cannot be today");

        }
    }
}
