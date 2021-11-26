using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentApp.Models;
using RecruitmentApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentApp.Controllers
{
    [Route("api/experience")]
    [ApiController]
    [Authorize]
    public class ExperienceController : ControllerBase
    {
        private IExperienceService _experienceService;

        public ExperienceController(IExperienceService experience)
        {
            _experienceService = experience;
        }
        [HttpPost]
        public ActionResult Create([FromBody] CreateExperienceDto dto)
        {
            var id = _experienceService.Create(dto);

            return Ok();
        }

        [HttpGet]
        public ActionResult<List<ExperienceDto>> GetAll()
        {
            var experienceDtos = _experienceService.GetAll();
            return Ok(experienceDtos);
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromBody] UpdateExperienceDto dto, [FromRoute] int id)
        {
            _experienceService.Update(id, dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            _experienceService.Delete(id);

            return NoContent();
        }
        [HttpGet("{id}")]
        public ActionResult<ResumeDto> Get([FromRoute] int id)
        {
            var experience = _experienceService.GetById(id);
            return Ok(experience);
        }
    }
}
