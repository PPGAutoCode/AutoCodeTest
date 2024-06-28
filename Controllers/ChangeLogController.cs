
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChangeLogController : ControllerBase
    {
        private readonly IChangeLogService _changeLogService;

        public ChangeLogController(IChangeLogService changeLogService)
        {
            _changeLogService = changeLogService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateChangeLog([FromBody] Request<CreateChangeLogDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _changeLogService.CreateChangeLog(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetChangeLog([FromBody] Request<ChangeLogRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _changeLogService.GetChangeLog(request.Payload);
                return Ok(new Response<ChangeLog> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateChangeLog([FromBody] Request<UpdateChangeLogDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _changeLogService.UpdateChangeLog(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteChangeLog([FromBody] Request<DeleteChangeLogDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _changeLogService.DeleteChangeLog(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListChangeLog([FromBody] Request<ListChangeLogRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _changeLogService.GetListChangeLog(request.Payload);
                return Ok(new Response<List<ChangeLog>> { Payload = result });
            });
        }
    }
}
