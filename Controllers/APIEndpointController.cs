
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class APIEndpointController : ControllerBase
    {
        private readonly IAPIEndpointService _apiEndpointService;

        public APIEndpointController(IAPIEndpointService apiEndpointService)
        {
            _apiEndpointService = apiEndpointService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAPIEndpoint([FromBody] CreateAPIEndpointDto request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _apiEndpointService.CreateAPIEndpoint(request);
                return Ok(new Response<string> { Data = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetAPIEndpoint([FromBody] APIEndpointRequestDto request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _apiEndpointService.GetAPIEndpoint(request);
                return Ok(new Response<APIEndpoint> { Data = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateAPIEndpoint([FromBody] UpdateAPIEndpointDto request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _apiEndpointService.UpdateAPIEndpoint(request);
                return Ok(new Response<string> { Data = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteAPIEndpoint([FromBody] DeleteAPIEndpointDto request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _apiEndpointService.DeleteAPIEndpoint(request);
                return Ok(new Response<bool> { Data = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListAPIEndpoint([FromBody] ListAPIEndpointRequestDto request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _apiEndpointService.GetListAPIEndpoint(request);
                return Ok(new Response<List<APIEndpoint>> { Data = result });
            });
        }
    }
}
