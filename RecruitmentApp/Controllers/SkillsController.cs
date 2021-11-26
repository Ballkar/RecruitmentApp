using Microsoft.AspNetCore.Mvc;
using RecruitmentApp.Models;
using RecruitmentApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentApp.Controllers
{
    [Route("api/skills")]
    [ApiController]
    public class SkillsController : ControllerBase
    {
        private readonly ISkillsService _skillsService;

        public SkillsController(ISkillsService skillsService)
        {
            _skillsService = skillsService;
        }
        [HttpPost]
        public ActionResult<List<SkillsDto>> Autocomplete([FromBody] SearchSkillsDto term)
        {
            return _skillsService.GetSkillsAutoomplete(term);
        }
    }
}
