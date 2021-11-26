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
    [Route("api/seniority")]
    [ApiController]

    public class SeniorityController : ControllerBase
    {
        private ISeniorityService _seniorityService;

        public SeniorityController(ISeniorityService seniorityService)
        {
            _seniorityService = seniorityService;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<SeniorityDto> GetSeniority([FromRoute] int id)
        {
            SeniorityDto resume = _seniorityService.GetSeniority(id);
            return Ok(resume);
        }
    }
}
