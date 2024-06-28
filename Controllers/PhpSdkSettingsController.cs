
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("PhpSdkSettings")]
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
}
