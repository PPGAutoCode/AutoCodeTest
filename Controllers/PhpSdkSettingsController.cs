
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhpSdkSettingsController : ControllerBase
    {
        private readonly IPhpSdkSettingsService _phpSdkSettingsService;

        public PhpSdkSettingsController(IPhpSdkSettingsService phpSdkSettingsService)
        {
            _phpSdkSettingsService = phpSdkSettingsService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePhpSdkSettings([FromBody] Request<CreatePhpSdkSettingsDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _phpSdkSettingsService.CreatePhpSdkSettings(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetPhpSdkSettings([FromBody] Request<PhpSdkSettingsRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _phpSdkSettingsService.GetPhpSdkSettings(request.Payload);
                return Ok(new Response<PhpSdkSettings> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdatePhpSdkSettings([FromBody] Request<UpdatePhpSdkSettingsDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _phpSdkSettingsService.UpdatePhpSdkSettings(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeletePhpSdkSettings([FromBody] Request<DeletePhpSdkSettingsDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _phpSdkSettingsService.DeletePhpSdkSettings(request.Payload);
                return Ok(new Response<bool> { Payload = result });
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
                        Code = ex is BusinessException || ex is TechnicalException ? ex.GetType().Name : "1001",
                        Description = ex is BusinessException || ex is TechnicalException ? ex.Message : "A technical exception has occurred, please contact your system administrator"
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
    }
}
