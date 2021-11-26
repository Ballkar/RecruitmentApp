using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecruitmentApp.Entities;
using RecruitmentApp.Exceptions;
using RecruitmentApp.Models;

namespace RecruitmentApp.Services
{
    public interface IAccountService
    {
        void RegisterUser(RegisterUserDto dto);
        JwtDto GenerateJwt(LoginDto dto);
        List<RoleDto> GetPublicRole();
        UserDto GetUser();
    }
    public class AccountService : IAccountService
    {
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;
        private readonly RecruitmentAppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthenticationSettings _authenticationSettings;


        public AccountService(RecruitmentAppDbContext context, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings, IMapper mapper, IUserContextService userContextService)
        {
            _mapper = mapper;
            _context = context;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
            _userContextService = userContextService;
        }
        public void RegisterUser(RegisterUserDto dto)
        {
            var newUser = new User()
            {
                Email = dto.Email,
                
                RoleId = dto.RoleId
            };
            var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);

            newUser.PasswordHash = hashedPassword;
            _context.Users.Add(newUser);
            _context.SaveChanges();
        }

        public JwtDto GenerateJwt(LoginDto dto)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Email == dto.Email);

            //if (user is null)
            //{
            //    throw new ValidationException("Invalid username or password");
            //}

            //var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            //if (result == PasswordVerificationResult.Failed)
            //{
            //    throw new ValidationException("Invalid username or password");
            //}

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();
            JwtDto jwtDto = new JwtDto();
            jwtDto.Token = tokenHandler.WriteToken(token);
            return jwtDto;

        }

        public List<RoleDto> GetPublicRole()
        {
            var roles = _context
                           .Roles
                           .Where(r => r.IsPublic == true)
                           .ToList();
            var rolesDtos = _mapper.Map<List<RoleDto>>(roles);
            return rolesDtos;
        }

        public UserDto GetUser()
        {
            var user = _context
                           .Users
                           .First(r => r.Id == _userContextService.GetUserId);
            return _mapper.Map<UserDto>(user);
        }
    }
}
