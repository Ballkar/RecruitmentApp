using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RecruitmentApp.Entities;
using RecruitmentApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentApp.Services
{
    public interface ISkillsService
    {
        List<SkillsDto> GetSkillsAutoomplete(SearchSkillsDto term);
    }

    public class SkillsService : ISkillsService
    {
        private readonly RecruitmentAppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<SkillsService> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        public SkillsService(RecruitmentAppDbContext dbContext, IMapper mapper, ILogger<SkillsService> logger, IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }

        public List<SkillsDto> GetSkillsAutoomplete(SearchSkillsDto term)
        {
            var skills = _dbContext
                            .Skills
                            .Where(o => o.Name.StartsWith(term.Name))
                            .ToList();
            var skillsDtos = _mapper.Map<List<SkillsDto>>(skills);
            return skillsDtos;
        }

    }
}
