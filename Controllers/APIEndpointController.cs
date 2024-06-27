
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
        public async Task<IActionResult> CreateAPIEndpoint([FromBody] CreateAPIEndpointDTO request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _apiEndpointService.CreateAPIEndpoint(request);
                return Ok(new Response<string>(result));
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetAPIEndpoint([FromBody] APIEndpointRequestDTO request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _apiEndpointService.GetAPIEndpoint(request);
                return Ok(new Response<APIEndpoint>(result));
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateAPIEndpoint([FromBody] UpdateAPIEndpointDTO request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _apiEndpointService.UpdateAPIEndpoint(request);
                return Ok(new Response<string>(result));
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteAPIEndpoint([FromBody] DeleteAPIEndpointDTO request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _apiEndpointService.DeleteAPIEndpoint(request);
                return Ok(new Response<bool>(result));
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListAPIEndpoint([FromBody] ListAPIEndpointRequestDTO request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _apiEndpointService.GetListAPIEndpoint(request);
                return Ok(new Response<List<APIEndpoint>>(result));
            });
        }
    }
}
