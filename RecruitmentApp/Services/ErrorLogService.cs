using Microsoft.AspNetCore.Http;
using RecruitmentApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentApp.Services
{
    public interface IErrorLogService
    {
        void CreateLog(string message, int statusCode, HttpContext request);
    }
    public class ErrorLogService : IErrorLogService
    {
        private readonly IUserContextService _userContextService;
        private readonly RecruitmentAppDbContext _dbContext;

        public ErrorLogService(IUserContextService userContextService, RecruitmentAppDbContext dbContext)
        {
            _userContextService = userContextService;
            _dbContext = dbContext;
        }

        public void CreateLog(string message, int statusCode, HttpContext request)
        {
            var errorLog = new ErrorLog();
            errorLog.User = _userContextService.GetUserId;
            errorLog.Message = message.ToString();
            errorLog.Method = request.Request.Method.ToString();
            errorLog.PathBase = request.Request.Path.ToString();
            errorLog.Headers = request.Request.Headers.ToString();

            errorLog.StatusCode = statusCode;
            errorLog.DateTime = DateTime.Now;

            _dbContext.ErrorLogs.Add(errorLog);
            _dbContext.SaveChanges();
        }
    }
}
