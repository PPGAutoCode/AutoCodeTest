
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
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
}
