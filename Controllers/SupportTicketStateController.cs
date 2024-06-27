
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
    public class SupportTicketStateController : ControllerBase
    {
        private readonly ISupportTicketStateService _supportTicketStateService;

        public SupportTicketStateController(ISupportTicketStateService supportTicketStateService)
        {
            _supportTicketStateService = supportTicketStateService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSupportTicketState([FromBody] Request<CreateSupportTicketStateDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _supportTicketStateService.CreateSupportTicketState(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetSupportTicketState([FromBody] Request<SupportTicketStateRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _supportTicketStateService.GetSupportTicketState(request.Payload);
                return Ok(new Response<SupportTicketState> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateSupportTicketState([FromBody] Request<UpdateSupportTicketStateDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _supportTicketStateService.UpdateSupportTicketState(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteSupportTicketState([FromBody] Request<DeleteSupportTicketStateDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _supportTicketStateService.DeleteSupportTicketState(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListSupportTicketState([FromBody] Request<ListSupportTicketStateRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = 
