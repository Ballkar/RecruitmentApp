using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using RecruitmentApp.Entities;

namespace RecruitmentApp.Models.Validators
{
    public class AddFeedbackDtoValidator : AbstractValidator<AddFeedbackDto>
    {
        public AddFeedbackDtoValidator(RecruitmentAppDbContext dbContext)
        {
            RuleFor(x => x.Message)
                .NotEmpty()
                .WithMessage("Message is empty")
                .WithErrorCode("422")
                .MinimumLength(50)
                .WithMessage("Message it's too short")
                .WithErrorCode("422")
                .MaximumLength(2100)
                .WithMessage("Message it's too long")
                .WithErrorCode("422");

            RuleFor(x => x.CompanyName)
                .NotEmpty()
                .WithMessage("Message is empty")
                .WithErrorCode("422")
                .MinimumLength(1)
                .WithMessage("CompanyName it's too short")
                .WithErrorCode("422");
        }
    }
}
