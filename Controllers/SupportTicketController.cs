
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("supportTicket")]
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
    }
}
