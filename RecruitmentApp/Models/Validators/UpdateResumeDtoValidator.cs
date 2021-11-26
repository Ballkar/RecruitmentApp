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
    public class UpdateResumeDtoValidator : AbstractValidator<UpdateResumeDto>
    {
        public UpdateResumeDtoValidator(RecruitmentAppDbContext dbContext)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Field is empty")
                .WithErrorCode("422");
            
            RuleFor(x => x.Surname)
                .NotEmpty()
                .WithMessage("Field is empty")
                .WithErrorCode("422");
            DateTime startDate = new DateTime();

            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .WithMessage("Date of birth is empty")
                .WithErrorCode("422")
                .Custom((value, context) =>
                {
                    startDate = value;
                });

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("Field is empty")
                .WithErrorCode("422");
            
            RuleFor(x => x.GithubUrl)
                .NotEmpty()
                .WithMessage("Field is empty")
                .WithErrorCode("422")
                .Custom((value, context) =>
                {
                    
                });

            RuleFor(x => x.Description)
                .MinimumLength(6)
                .WithMessage("Description it's too short")
                .WithErrorCode("422")
                .MaximumLength(50)
                .WithMessage("Description it's too long")
                .WithErrorCode("422");

            RuleFor(x => x.SeniorityId)
                .NotEmpty()
                .WithMessage("Field is empty")
                .WithErrorCode("422");

            RuleFor(x => x.SkillsId)
                .Must(list => list.Count > 0)
                .WithMessage("The list must contain at least one item ");
        }
    }
}
