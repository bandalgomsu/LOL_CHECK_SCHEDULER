using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace lol_check_scheduler.src.common.exception
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            this._next = next;
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"PATH = {context.Request.Path}");
                await HandleExceptionsAsync(context, ex);
            }
        }

        private Task HandleExceptionsAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = CommonErrorCode.INTERNAL_SERVER_ERROR.Status;

            var errorResponse = new ErrorResponse
            {
                Code = CommonErrorCode.INTERNAL_SERVER_ERROR.Code,
                Message = CommonErrorCode.INTERNAL_SERVER_ERROR.Message,
                Status = CommonErrorCode.INTERNAL_SERVER_ERROR.Status,
            };

            switch (ex)
            {
                case BusinessException businessException:
                    context.Response.StatusCode = businessException.ErrorCode.Status;

                    errorResponse.Code = businessException.ErrorCode.Code;
                    errorResponse.Message = businessException.ErrorCode.Message;
                    errorResponse.Status = businessException.ErrorCode.Status;

                    break;
                default:
                    break;
            }

            string response = JsonSerializer.Serialize(errorResponse);

            return context.Response.WriteAsync(response);
        }
    }

}