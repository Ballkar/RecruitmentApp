using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using RecruitmentApp.Entities;
using RecruitmentApp.Exceptions;
using RecruitmentApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RecruitmentApp.Services
{
    public interface ISeniorityService
    {
        SeniorityDto GetSeniority(int id);
    }
    public class SeniorityService : ISeniorityService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly RecruitmentAppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<ResumeService> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        public SeniorityService(IHttpClientFactory clientFactory, RecruitmentAppDbContext dbContext, IMapper mapper, ILogger<ResumeService> logger, IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _clientFactory = clientFactory;
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }
        public SeniorityDto GetSeniority(int id)
        {
            var seniorities = _dbContext
                .Seniority.Where(r => r.id == id).First();
            if (seniorities is null)
                throw new NotFoundException("Seniority not found");
            var seniority = _mapper.Map<SeniorityDto>(seniorities);
            return seniority;
        }
    }
    
}
