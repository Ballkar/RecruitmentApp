using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RecruitmentApp.Exceptions;
using RecruitmentApp.Services;

namespace RecruitmentApp.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IErrorLogService _errorLogService;

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger, IErrorLogService errorLogService)
        {
            _logger = logger;
            _errorLogService = errorLogService;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (ForbidException forbidException)
            {
                _errorLogService.CreateLog(forbidException.Message, 403, context);
                context.Response.StatusCode = 403;
            }
            catch (BadRequestException badRequestException)
            {
                _errorLogService.CreateLog(badRequestException.Message, 400, context);
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(badRequestException.Message);
            }
            catch (NotFoundException notFoundException)
            {
                _errorLogService.CreateLog(notFoundException.Message, 404, context);
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(notFoundException.Message);
            }
            catch (ValidationException validationException)
            {
                _errorLogService.CreateLog(validationException.Message, 422, context);
                context.Response.StatusCode = 422;
                await context.Response.WriteAsync(validationException.Message);
            }

            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                _errorLogService.CreateLog(e.Message, 500, context);
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Something went wrong");
            }
        }
    }
}
