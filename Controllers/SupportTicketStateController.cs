
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
                var result = await _supportTicketStateService.GetListSupportTicketState(request.Payload);
                return Ok(new Response<List<SupportTicketState>> { Payload = result });
            });
        }
    }

    public static class SafeExecutor
    {
        public static async Task<IActionResult> ExecuteAsync(Func<Task<IActionResult>> action)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                var exceptionResponse = new Response<object>
                {
                    Exception = new ExceptionInfo
                    {
                        Id = Guid.NewGuid().ToString(),
                        Code = "1001",
                        Description = "A technical exception has occurred, please contact your system administrator"
                    }
                };
                return new OkObjectResult(exceptionResponse);
            }
        }
    }

    public class Request<T>
    {
        public Header Header { get; set; }
        public T Payload { get; set; }
    }

    public class Response<T>
    {
        public T Payload { get; set; }
        public ExceptionInfo Exception { get; set; }
    }

    public class ExceptionInfo
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class Header
    {
        public string ID { get; set; }
        public string Application { get; set; }
        public string Bank { get; set; }
        public string UserId { get; set; }
    }
}
