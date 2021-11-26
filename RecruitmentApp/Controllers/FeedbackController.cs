using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RecruitmentApp.Entities;
using RecruitmentApp.Models;
using RecruitmentApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentApp.Controllers
{
    [Route("api/Feedback")]
    [ApiController]
    [Authorize]

    public class FeedbackController : ControllerBase
    {
        private IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult<SeniorityDto> AddFeedback([FromBody] AddFeedbackDto addFeedbackDto)
        {
            bool result = _feedbackService.AddFeedback(addFeedbackDto);
            return Ok(result);
        }

    }
}
