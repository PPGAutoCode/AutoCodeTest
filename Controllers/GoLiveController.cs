
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("goLive")]
    public class GoLiveController : ControllerBase
    {
        private readonly IGoLiveService _goLiveService;

        public GoLiveController(IGoLiveService goLiveService)
        {
            _goLiveService = goLiveService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGoLive([FromBody] Request<CreateGoLiveDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                string result = await _goLiveService.CreateGoLive(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }
    }
}
