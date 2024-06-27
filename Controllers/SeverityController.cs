
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
    public class SeverityController : ControllerBase
    {
        private readonly ISeverityService _severityService;

        public SeverityController(ISeverityService severityService)
        {
            _severityService = severityService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSeverity([FromBody] Request<CreateSeverityDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _severityService.CreateSeverity(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetSeverity([FromBody] Request<SeverityRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _severityService.GetSeverity(request.Payload);
                return Ok(new Response<Severity> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateSeverity([FromBody] Request<UpdateSeverityDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _severityService.UpdateSeverity(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteSeverity([FromBody] Request<DeleteSeverityDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _severityService.DeleteSeverity(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListSeverity([FromBody] Request<ListSeverityRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _severityService.GetListSeverity(request.Payload);
                return Ok(new Response<List<Severity>> { Payload = result });
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
                return new OkObjectResult(new Response<object>
                {
                    Exception = new ExceptionInfo
                    {
                        Id = Guid.NewGuid().ToString(),
                        Code = "1001",
                        Description = "A technical exception has occurred, please contact your system administrator"
                    }
                });
            }
        }
    }

    public class Request<T>
    {
        public Header Header { get; set; }
        public T Payload { get; set; }
    }

    public class Response<T>
    {
        public T Payload { get; set; }
        public ExceptionInfo Exception { get; set; }
    }

    public class Header
    {
        public string ID { get; set; }
        public string Application { get; set; }
        public string Bank { get; set; }
        public string UserId { get; set; }
    }

    public class ExceptionInfo
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
