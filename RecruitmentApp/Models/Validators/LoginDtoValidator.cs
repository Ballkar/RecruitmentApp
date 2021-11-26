using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using RecruitmentApp.Entities;
using Microsoft.AspNetCore.Identity;


namespace RecruitmentApp.Models.Validators
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        string email;
        public LoginDtoValidator(RecruitmentAppDbContext dbContext, IPasswordHasher<User> passwordHasher)
        {
            _passwordHasher = passwordHasher;

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is empty")
                .WithErrorCode("422")
                .EmailAddress()
                .WithMessage("Email is invalid")
                .WithErrorCode("422")
                .Custom((value, context) =>
                {
                    var emailInUse = dbContext.Users.Any(u => u.Email == value);
                    if (emailInUse)
                    {
                        email = value;
                    }
                    else
                    {
                        context.AddFailure("Email", "Invalid username or password");
                    }
                });
                

            RuleFor(x => x.Password)
                .MinimumLength(6)
                .WithMessage("Password it's too short")
                .WithErrorCode("422")
                .Custom((value, context) =>
                {
                    if (email!=null)
                    {
                        var user = dbContext.Users
                        .FirstOrDefault(u => u.Email == email);

                        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, value);
                        if (result == PasswordVerificationResult.Failed)
                        {
                            context.AddFailure("Email", "Invalid username or password");
                        }
                    }
                    else
                    {
                        context.AddFailure("Email", "Invalid username or password");
                    }
                });
        }
    }
}
