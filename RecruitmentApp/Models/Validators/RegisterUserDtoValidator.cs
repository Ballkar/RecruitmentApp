using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using RecruitmentApp.Entities;

namespace RecruitmentApp.Models.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator(RecruitmentAppDbContext dbContext)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is empty")
                .WithErrorCode("422")
                .EmailAddress()
                .WithMessage("Email is invalid")
                .WithErrorCode("422");

            RuleFor(x => x.Password)
                .MinimumLength(6)
                .WithMessage("Password it's too short")
                .WithErrorCode("422");

            RuleFor(x => x.ConfirmPassword)
                .Equal(e => e.Password)
                .WithMessage("The value of the 'Confirm Password' field must be equal to Password")
                .WithErrorCode("422");

            RuleFor(x => x.Email)
                .Custom((value, context) =>
                {
                    var emailInUse = dbContext.Users.Any(u => u.Email == value);
                    if (emailInUse)
                    {
                        context.AddFailure("Email", "That email is taken");
                    }
                });
            RuleFor(x => x.RoleId)
                .Custom((value, context) =>
                {
                    var roleNotAvaileble = dbContext.Roles.Any(u => u.Id == value && u.IsPublic == true);
                    if (!roleNotAvaileble)
                    {
                        context.AddFailure("Role", "Role type invalid or does not exist");
                    }
                });
        }
    }
}
