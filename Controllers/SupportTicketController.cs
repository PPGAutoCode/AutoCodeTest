
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
    public class SupportTicketController : ControllerBase
    {
        private readonly ISupportTicketService _supportTicketService;

        public SupportTicketController(ISupportTicketService supportTicketService)
        {
            _supportTicketService = supportTicketService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSupportTicket([FromBody] Request<CreateSupportTicketDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _supportTicketService.CreateSupportTicket(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetSupportTicket([FromBody] Request<RequestSupportTicketDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _supportTicketService.GetSupportTicket(request.Payload);
                return Ok(new Response<SupportTicket> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateSupportTicket([FromBody] Request<UpdateSupportTicketDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _supportTicketService.UpdateSupportTicket(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteSupportTicket([FromBody] Request<DeleteSupportTicketDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _supportTicketService.DeleteSupportTicket(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListSupportTicket([FromBody] Request<ListSupportTicketRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _supportTicketService.GetListSupportTicket(request.Payload);
                return Ok(new Response<List<SupportTicket>);
            });
        }
    }
}
