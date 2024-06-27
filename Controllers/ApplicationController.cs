
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public ApplicationController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateApplication([FromBody] Request<CreateApplicationDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _applicationService.CreateApplication(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetApplication([FromBody] Request<ApplicationRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _applicationService.GetApplication(request.Payload);
                return Ok(new Response<Application> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateApplication([FromBody] Request<UpdateApplicationDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _applicationService.UpdateApplication(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteApplication([FromBody] Request<DeleteApplicationDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _applicationService.DeleteApplication(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListApplication([FromBody] Request<ListApplicationRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _applicationService.GetListApplication(request.Payload);
                return Ok(new Response<List<Application>> { Payload = result });
            });
        }
    }

    public static class SafeExecutor
    {
        public static async Task<IActionResult> ExecuteAsync(Func<Task<IActionResult>> action)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                var exceptionResponse = new Response<object>
                {
                    Exception = new ExceptionInfo
                    {
                        Id = Guid.NewGuid().ToString(),
                        Code = ex is BusinessException || ex is TechnicalException ? ex.GetType().Name : "1001",
                        Description = ex is BusinessException || ex is TechnicalException ? ex.Message : "A technical exception has occurred, please contact your system administrator"
                    }
                };
                return new OkObjectResult(exceptionResponse);
            }
        }
    }

    public class Response<T>
    {
        public T Payload { get; set; }
        public ExceptionInfo Exception { get; set; }
    }

    public class ExceptionInfo
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class Request<T>
    {
        public Header Header { get; set; }
        public T Payload { get; set; }
    }

    public class Header
    {
        public string ID { get; set; }
        public string Application { get; set; }
        public string Bank { get; set; }
        public string UserId { get; set; }
    }

    public class BusinessException : Exception
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class TechnicalException : Exception
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
