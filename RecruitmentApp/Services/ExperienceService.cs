using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using RecruitmentApp.Authorization;
using RecruitmentApp.Entities;
using RecruitmentApp.Exceptions;
using RecruitmentApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RecruitmentApp.Services
{
    public interface IExperienceService
    {
        object Create(CreateExperienceDto dto);
        object GetAll();
        void Update(int id, UpdateExperienceDto dto);
        void Delete(int id);
        object GetById(int id);
    }
    public class ExperienceService : IExperienceService
    {
        private IMapper _mapper;
        private readonly IUserContextService _userContextService;
        private readonly IAuthorizationService _authorizationService;
        private RecruitmentAppDbContext _dbContext;
        private AuthenticationSettings _authenticationSettings;
        private readonly ILogger<ResumeService> _logger;

        public ExperienceService(RecruitmentAppDbContext context, AuthenticationSettings authenticationSettings, ILogger<ResumeService> logger, IMapper mapper, IUserContextService userContextService, RecruitmentAppDbContext dbContext, IAuthorizationService authorizationService)
        {
            _mapper = mapper;
            _userContextService = userContextService;
            _authorizationService = authorizationService;
            _dbContext = context;
            _authenticationSettings = authenticationSettings;
            _logger = logger;
        }

        public object Create(CreateExperienceDto dto)
        {
            var experience = _mapper.Map<Experience>(dto);
            experience.UserId = _userContextService.GetUserId;
            _dbContext.Experiences.Add(experience);
            _dbContext.SaveChanges();

            return experience.id;
        }

        public void Delete(int id)
        {
            _logger.LogError($"Experience with id: {id} DELETE action invoked");

            var experience = _dbContext
                .Experiences
                .FirstOrDefault(r => r.id == id);

            if (experience is null)
                throw new NotFoundException("Experience not found");

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, experience,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException("You don't have permission to access this resource");
            }
            _dbContext.Experiences.Remove(experience);
            _dbContext.SaveChanges();
        }

        public object GetAll()
        {
            var experiences = _dbContext
                .Experiences
                .Where(r => r.UserId == _userContextService.GetUserId).ToList();

            var result = _mapper.Map<List<ExperienceDto>>(experiences);

            return result;
        }

        public object GetById(int id)
        {
            var experience = _dbContext
                .Experiences
                .FirstOrDefault(r => r.id == id && r.UserId == _userContextService.GetUserId);

            if (experience is null)
                throw new NotFoundException("Experience not found");

            var result = _mapper.Map<ExperienceDto>(experience);
            return result;
        }

        public void Update(int id, UpdateExperienceDto dto)
        {
            var experience = _dbContext
                .Experiences
                .FirstOrDefault(r => r.id == id);

            if (experience is null)
                throw new NotFoundException("Experience not found");


            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, experience,
                new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException("You don't have permission to access this resource");
            }

            experience.Name = dto.Name;
            experience.Description = dto.Description;
            experience.StartDate = dto.StartDate;
            experience.EndDate = dto.EndDate;

            _dbContext.SaveChanges();
        }
    }
}
