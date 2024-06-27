
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
    public class SelectTicketPriorityController : ControllerBase
    {
        private readonly ISelectTicketPriorityService _service;

        public SelectTicketPriorityController(ISelectTicketPriorityService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSelectTicketPriority([FromBody] Request<CreateSelectTicketPriorityDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _service.CreateSelectTicketPriority(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetSelectTicketPriority([FromBody] Request<SelectTicketPriorityRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _service.GetSelectTicketPriority(request.Payload);
                return Ok(new Response<SelectTicketPriority> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateSelectTicketPriority([FromBody] Request<UpdateSelectTicketPriorityDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _service.UpdateSelectTicketPriority(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteSelectTicketPriority([FromBody] Request<DeleteSelectTicketPriorityDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _service.DeleteSelectTicketPriority(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListSelectTicketPriority([FromBody] Request<ListSelectTicketPriorityRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _service.GetGetListSelectTicketPriority(request.Payload);
                return Ok(new Response<List<SelectTicketPriority>> { Payload = result });
            });
        }
    }
}
