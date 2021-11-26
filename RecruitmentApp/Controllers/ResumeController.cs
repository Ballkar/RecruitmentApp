using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentApp.Entities;
using RecruitmentApp.Models;
using RecruitmentApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentApp.Controllers
{
    [Route("api/resume")]
    [ApiController]
    [Authorize]

    public class ResumeController : ControllerBase
    {
        private readonly IResumeService _resumeService;

        public ResumeController(IResumeService resumeService)
        {
            _resumeService = resumeService;
        }

        [HttpPost]
        public ActionResult CreateResume([FromBody] CreateResumeDto dto)
        {
            var id = _resumeService.Create(dto);

            return Created($"/api/resume/{id}", null);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<IEnumerable<ResumeDtoList>> GetAll([FromQuery] ResumeQuery query)
        {
            var resumeDtos = _resumeService.GetAll(query);

            return Ok(resumeDtos);
        }
        [HttpGet("/api/resume/GetUserResumes")]
        public ActionResult<IEnumerable<ResumeDtoList>> GetUserResumes([FromQuery] ResumeQuery query)
        {
            var resumeDtos = _resumeService.GetUserResumes(query);

            return Ok(resumeDtos);
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromBody] UpdateResumeDto dto, [FromRoute] int id)
        {

            _resumeService.Update(id, dto);

            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            _resumeService.Delete(id);

            return NoContent();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Resume>> Get([FromRoute] int id)
        {
            var resume = await _resumeService.GetByIdAsync(id);
            return Ok(resume);
        }
        [HttpPost("/api/resume/AttachExperience")]
        public ActionResult AttachExperience([FromBody] AttachExperienceDto dto)
        {
            _resumeService.AttachExperience(dto);

            return Ok();
        }
        [HttpPost("/api/resume/DetachExperience")]
        public ActionResult DetachExperience([FromBody] DetachExperienceDto dto)
        {
            _resumeService.DetachExperience(dto);

            return Ok();
        }
    }
}

