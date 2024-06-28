
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("PhpSettingSdk")]
    public class PhpSettingSdkController : ControllerBase
    {
        private readonly IPhpSettingsSdkService _phpSettingsSdkService;

        public PhpSettingSdkController(IPhpSettingsSdkService phpSettingsSdkService)
        {
            _phpSettingsSdkService = phpSettingsSdkService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePhpSettingSdk([FromBody] Request<CreatePhpSettingSdkDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _phpSettingsSdkService.CreatePhpSettingsSdk(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetPhpSettingSdk([FromBody] Request<PhpSettingsSdkRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _phpSettingsSdkService.GetPhpSettingsSdk(request.Payload);
                return Ok(new Response<PhpSettingsSdk> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdatePhpSettingSdk([FromBody] Request<UpdatePhpSettingSdkDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _phpSettingsSdkService.UpdatePhpSettingsSdk(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeletePhpSettingSdk([FromBody] Request<DeletePhpSettingSdkDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _phpSettingsSdkService.DeletePhpSettingsSdk(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }
    }
}
