using AutoMapper;
using RecruitmentApp.Entities;
using RecruitmentApp.Models;

namespace RestaurantAPI
{
    public class RecruitmentAppMappingProfile : Profile
    {
        public RecruitmentAppMappingProfile()
        {
            CreateMap<Role, RoleDto>();
            CreateMap<Skill, SkillsDto>();

            CreateMap<Seniority, SeniorityDto>();
            CreateMap<SeniorityDto, Seniority>();
            
            CreateMap<CreateExperienceDto, Experience>();
            CreateMap<ExperienceDto, Experience>();
            CreateMap<Experience, ExperienceDto>();

            CreateMap<CreateResumeDto, Resume>();
            CreateMap<ResumeDto, Resume>();
            CreateMap<Resume, ResumeDto>();
            
            CreateMap<UserDto, User>();
            CreateMap<User, UserDto>();

            CreateMap<AddFeedbackDto, Feedback>();
            CreateMap<Feedback, AddFeedbackDto>();
            
            CreateMap<ResumeDtoList, Resume>();
            CreateMap<Resume, ResumeDtoList>();
        }
    }
}
