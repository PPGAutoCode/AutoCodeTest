
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnvironmentController : ControllerBase
    {
        private readonly IEnvironmentService _environmentService;

        public EnvironmentController(IEnvironmentService environmentService)
        {
            _environmentService = environmentService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateEnvironment([FromBody] Request<CreateEnvironmentDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _environmentService.CreateEnvironment(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetEnvironment([FromBody] Request<EnvironmentRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _environmentService.GetEnvironment(request.Payload);
                return Ok(new Response<Environment> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateEnvironment([FromBody] Request<UpdateEnvironmentDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _environmentService.UpdateEnvironment(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteEnvironment([FromBody] Request<DeleteEnvironmentDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _environmentService.DeleteEnvironment(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListEnvironment([FromBody] Request<ListEnvironmentRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _environmentService.GetListEnvironment(request.Payload);
                return Ok(new Response<List<Environment>> { Payload = result });
            });
        }
    }
}
