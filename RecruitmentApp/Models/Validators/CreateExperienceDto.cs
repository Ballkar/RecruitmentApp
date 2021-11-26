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
    public class CreateExperienceDtoValidator : AbstractValidator<CreateExperienceDto>
    {
        public CreateExperienceDtoValidator(RecruitmentAppDbContext dbContext)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Field is empty")
                .WithErrorCode("422");


            RuleFor(x => x.Description)
                .MinimumLength(6)
                .WithMessage("Description it's too short")
                .WithErrorCode("422")
                .MaximumLength(50)
                .WithMessage("Description it's too long")
                .WithErrorCode("422");

            DateTime startDate = new DateTime();

            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("Start Date is empty")
                .WithErrorCode("422")
                .Custom((value, context) =>
                {
                    startDate = value;
                });
            
            RuleFor(x => x.EndDate)
                .NotEmpty()
                .WithMessage("End Date is empty")
                .WithErrorCode("422")
                .Custom((value, context) =>
                {
                    if(startDate > value)
                        context.AddFailure("EndDate", "End date cannot be earlier than the start date");
                });
            
        }
    }
}
