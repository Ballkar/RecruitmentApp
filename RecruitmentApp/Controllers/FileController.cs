using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using RecruitmentApp.Authorization;
using RecruitmentApp.Entities;
using RecruitmentApp.Exceptions;
using RecruitmentApp.Services;

namespace RestaurantAPI.Controllers
{
    [Route("file")]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly IUserContextService _userContextService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IFileService _fileService;
        private readonly RecruitmentAppDbContext _dbContext;

        public FileController(IUserContextService userContextService, RecruitmentAppDbContext dbContext, IAuthorizationService authorizationService, IFileService fileService)
        {
            _authorizationService = authorizationService;
            _fileService = fileService;
            _dbContext = dbContext;
            _userContextService = userContextService;
        }
        [HttpGet]
        [ResponseCache(Duration = 1200, VaryByQueryKeys = new[] { "fileName" })]
        public ActionResult GetFile([FromQuery] int resumeId)
        {
            var resume = _dbContext
               .Resumes
               .FirstOrDefault(r => r.id == resumeId);

            if (resume is null)
                throw new NotFoundException("Resume not found");


            var rootPath = Directory.GetCurrentDirectory();

            var filePath = $"{_fileService.GetUserFilesUrl(resume.UserId)}{resume.ImageName}";

            var fileExists = System.IO.File.Exists(filePath);
            if (!fileExists)
            {
                return NotFound();
            }

            var contentProvider = new FileExtensionContentTypeProvider();
            contentProvider.TryGetContentType(resume.ImageName, out string contentType);

            var fileContents = System.IO.File.ReadAllBytes(filePath);

            return File(fileContents, contentType, resume.ImageName);
        }

        [HttpPost]
        public ActionResult Upload([FromForm] IFormFile file, [FromQuery] int resumeId)
        {
            if (file != null && file.Length > 0)
            {
                return BadRequest();
            }

            var resume = _dbContext
            .Resumes
            .FirstOrDefault(r => r.id == resumeId);

            if (resume is null)
                throw new NotFoundException("Resume not found");

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, resume,
            new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException("You don't have permission to access this resource");
            }
            var fileName = file.FileName;
            var fullPath = $"{_fileService.GetUserFilesUrl(_userContextService.GetUserId)} {fileName}";
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
                resume.ImageName = file.FileName;
                _dbContext.SaveChanges();
            }
            return Ok();

        }
    }
}
