using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentApp.Entities;
using RecruitmentApp.Models;
using RecruitmentApp.Services;

namespace RecruitmentApp.Controllers
{
    [Route("api/account")]
    [ApiController]

    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        
        [HttpPost("register")]
        public ActionResult RegisterUser([FromBody]RegisterUserDto dto)
        {
            _accountService.RegisterUser(dto);
            return Ok();
        }
        
        [HttpPost("login")]
        public ActionResult Login([FromBody]LoginDto dto)
        {
            JwtDto token = _accountService.GenerateJwt(dto);
            return Ok(token);
        }
        [HttpGet("role")]
        public ActionResult GetRole()
        {
            List<RoleDto> roles = _accountService.GetPublicRole();
            return Ok(roles);
        }

        [Authorize]
        [HttpGet("user")]
        public ActionResult GetUser()
        {
            UserDto user = _accountService.GetUser();
            return Ok(user);
        }
    }
}
