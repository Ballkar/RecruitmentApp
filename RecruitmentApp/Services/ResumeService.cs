using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using RecruitmentApp.Entities;
using RecruitmentApp.Exceptions;
using RecruitmentApp.Models;
using RecruitmentApp.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;

namespace RecruitmentApp.Services
{
    public interface IResumeService
    {
        object Create(CreateResumeDto dto);
        void Update(int id, UpdateResumeDto dto);
        PagedResult<ResumeDtoList> GetAll(ResumeQuery query);
        void Delete(int id);
        Task<ResumeDto> GetByIdAsync(int id);
        void AttachExperience(AttachExperienceDto dto);
        void DetachExperience(DetachExperienceDto dto);
        PagedResult<ResumeDtoList> GetUserResumes(OnlyPaginationQuery query);
    }
    public class ResumeService : IResumeService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly RecruitmentAppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<ResumeService> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        public ResumeService(IHttpClientFactory clientFactory, RecruitmentAppDbContext dbContext, IMapper mapper, ILogger<ResumeService> logger, IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _clientFactory = clientFactory;
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }
        public object Create(CreateResumeDto dto)
        {
            var resume = _mapper.Map<Resume>(dto);

            resume.UserId = _userContextService.GetUserId;

            if(resume.GithubUrl!=null)
            resume.GithubUrl = ChangeUrlToAcceptable(resume.GithubUrl);

            _dbContext.Resumes.Add(resume);
            _dbContext.SaveChanges();


            var addedResume = _dbContext
                .Resumes
                .Include(s => s.Skills)
                .OrderByDescending(r => r.id)
                .FirstOrDefault(r => r.UserId == _userContextService.GetUserId);

            foreach (var item in dto.SkillsId)
            {
                var _resumeSkill = new ResumeSkill { ResumeId = addedResume.id, SkillId = item };

                addedResume.Skills.Add(_resumeSkill);
                _dbContext.SaveChanges();
            }

            return resume.id;
        }

        string ChangeUrlToAcceptable(string value)
        {
            if (value.ToLower().Contains("https://github.com/"))
            {
                string login = value.Replace("https://github.com/","");
                value = "https://api.github.com/users/" + login;
            }
            else if (value.ToLower().Contains("https://api.github.com/users/"))
            {
                string login = value.Replace("https://api.github.com/users/", "");
                value = "https://api.github.com/users/" + login;
            }
            return value;
        }

        public void Delete(int id)
        {
            _logger.LogError($"Resume with id: {id} DELETE action invoked");

            var resume = _dbContext
                .Resumes
                .FirstOrDefault(r => r.id == id);

            if (resume is null)
                throw new NotFoundException("Resume not found");

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, resume,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException("You don't have permission to access this resource");
            }

            _dbContext.Resumes.Remove(resume);
            _dbContext.SaveChanges();
        }
        public async Task<ResumeDto> GetByIdAsync(int id)
        {
            var resume = _dbContext
                .Resumes.Where(r => r.id == id)
                .FirstOrDefault();

            if (resume is null)
                throw new NotFoundException("Resume not found");

            var result = _mapper.Map<ResumeDto>(resume);

            if (resume.GithubUrl != null)
                result.GitData = await GetGithubDataFromUrl(resume.GithubUrl);
            result.SeniorityDto = _mapper.Map<SeniorityDto>(_dbContext.Seniority.First(s => s.id == resume.SeniorityId));
            result.Skills = _dbContext.Skills.Where(s => s.Resumes.Any(r => r.ResumeId == id)).ToList();
            result.Experiences = _dbContext.Experiences.Where(s => s.Resumes.Any(r => r.ResumeId == id)).ToList();
            return result;
        }
        async Task<GitDataDto> GetGithubDataFromUrl(string url)
        {
            if (ValidateUri(url) && IsGithubUrl(url))
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);

                var client = _clientFactory.CreateClient();

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.UserAgent.TryParseAdd("Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:22.0) Gecko/20100101 Firefox/22.0");

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var gitData = await response.Content.ReadFromJsonAsync<GitDataDto>();
                    return gitData;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        public void Update(int id, UpdateResumeDto dto)
        {
            var resume = _dbContext
                .Resumes
                .FirstOrDefault(r => r.id == id);

            if (resume is null)
                throw new NotFoundException("Resume not found");


            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, resume,
                new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException("You don't have permission to access this resource");
            }

            resume.Name = dto.Name;
            resume.Surname = dto.Surname;
            resume.DateOfBirth = dto.DateOfBirth;
            resume.City = dto.City;
            resume.GithubUrl = dto.GithubUrl;
            resume.Description = dto.Description;

            _dbContext.SaveChanges();
        }
        public PagedResult<ResumeDtoList> GetAll(ResumeQuery query)
        {
            var baseQuery = _dbContext
                .Resumes
                .AsQueryable();

            if (!string.IsNullOrEmpty(query.SearchPhrase))
            {
                var search = query.SearchPhrase.ToLower();
                baseQuery = baseQuery.Where(r => r.Name.ToLower().Contains(search) || r.Surname.ToLower().Contains(search));
            }

            if (!string.IsNullOrEmpty(query.Location))
            {
                var location = query.Location.ToLower();
                baseQuery = baseQuery.Where(r => r.City.ToLower().Contains(location));
            }

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                var columnsSelectors = new Dictionary<string, Expression<Func<Resume, object>>>
                {
                    { nameof(Resume.Name), r => r.Name },
                    { nameof(Resume.Surname), r => r.Surname }
                };

                var selectedColumn = columnsSelectors[query.SortBy];

                baseQuery = (query.SortDirection == SortDirection.ASC
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn));
            }

            var totalItemsCount = baseQuery.Count();
            
            var CurrentPage = Convert.ToBoolean(query.PageNumber) ? query.PageNumber : 1;
            var PageSize = Convert.ToBoolean(query.PageSize) ? query.PageSize : 5;
            var resumes = baseQuery
                .Skip(PageSize * (CurrentPage - 1))
                .Take(PageSize)
                .ToList();


            var resumeDtos = _mapper.Map<List<ResumeDtoList>>(resumes);

            var result = new PagedResult<ResumeDtoList>(resumeDtos, totalItemsCount, query.PageSize, query.PageNumber);

            return result;
        }
        public bool ValidateUri(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return true;
            }
            return Uri.TryCreate(uri, UriKind.Absolute, out _);
        }

        public bool IsGithubUrl(string url)
        {
            Uri _Url = new Uri(url);

            if (Uri.IsWellFormedUriString(url, UriKind.Absolute) && _Url.Host == "api.github.com")
            {
                return true;
            }
            else
            {
                throw new ValidationException("Invalid github URL");
            }
        }
        public void AttachExperience(AttachExperienceDto dto)
        {
            var resume = _dbContext
                .Resumes
                .Include(r => r.Experiences)
                .First(r => r.id == dto.ResumeId);

            if (resume is null)
                throw new NotFoundException("Resume not found");
            var experiences = _dbContext
                .Experiences
                .FirstOrDefault(r => r.id == dto.ExperienceId);
            if (experiences is null)
                throw new NotFoundException("Experience not found");

            var _experienceResume = new ExperienceResume { ResumeId = dto.ResumeId, ExperienceId = dto.ExperienceId };

            resume.Experiences.Add(_experienceResume);
            _dbContext.SaveChanges();
        }
        public void DetachExperience(DetachExperienceDto dto)
        {
            var resume = _dbContext
                .Resumes
                .Include(r => r.Experiences)
                .First(r => r.id == dto.ResumeId);

            if (resume is null)
                throw new NotFoundException("Resume not found");

            var _resumeExperience = resume.Experiences.FirstOrDefault(s => s.ExperienceId == dto.ExperienceId);
            if (_resumeExperience is null)
                throw new NotFoundException("Experience not found");

            resume.Experiences.Remove(_resumeExperience);
            _dbContext.SaveChanges();
        }

        public PagedResult<ResumeDtoList> GetUserResumes(OnlyPaginationQuery query)
        {
            var baseQuery = _dbContext
                .Resumes
                .Where(r => r.UserId == _userContextService.GetUserId);

            var resumes = baseQuery
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize)
                .ToList();

            var totalItemsCount = baseQuery.Count();

            var resumeDtos = _mapper.Map<List<ResumeDtoList>>(resumes);

            var result = new PagedResult<ResumeDtoList>(resumeDtos, totalItemsCount, query.PageSize, query.PageNumber);

            return result;
        }
    }
}
