using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using lol_check_scheduler.src.common.exception;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace test.src.common.exception
{
    public class ExceptionMiddlewareTest
    {
        private readonly Mock<ILogger<ExceptionMiddleware>> _logger = new Mock<ILogger<ExceptionMiddleware>>();
        private readonly Mock<RequestDelegate> _delegate = new Mock<RequestDelegate>();

        private readonly ExceptionMiddleware _exceptionMiddleware;

        public ExceptionMiddlewareTest()
        {
            _exceptionMiddleware = new ExceptionMiddleware(_delegate.Object, _logger.Object);
        }

        [Fact(DisplayName = "INVOKE_SUCCESS")]
        public async Task INVOKE_SUCCESS()
        {
            _delegate.Setup(d => d(It.IsAny<HttpContext>()))
                .Returns((HttpContext ctx) =>
                    {
                        ctx.Response.StatusCode = StatusCodes.Status200OK;
                        return Task.CompletedTask;
                    });

            var context = new DefaultHttpContext();
            await _exceptionMiddleware.InvokeAsync(context);


            Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        }

        [Fact(DisplayName = "INVOKE_SUCCESS_THROW_BY_BUSINESS_EXCEPTION")]
        public async Task INVOKE_SUCCESS_THROW_BY_BUSINESS_EXCEPTION()
        {
            _delegate.Setup(d => d(It.IsAny<HttpContext>()))
                .ThrowsAsync(new BusinessException(CommonErrorCode.INVALID_INPUT_VALUE));

            var context = new DefaultHttpContext();

            var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            await _exceptionMiddleware.InvokeAsync(context);

            memoryStream.Seek(0, SeekOrigin.Begin);
            var responseContent = await new StreamReader(context.Response.Body).ReadToEndAsync();

            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent)!;

            Assert.Equal(CommonErrorCode.INVALID_INPUT_VALUE.Code, errorResponse.Code);
            Assert.Equal(CommonErrorCode.INVALID_INPUT_VALUE.Message, errorResponse.Message);
            Assert.Equal(CommonErrorCode.INVALID_INPUT_VALUE.Status, errorResponse.Status);
        }

    }
}