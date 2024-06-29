
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;

        public OrganizationController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrganization([FromBody] Request<CreateOrganizationDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _organizationService.CreateOrganization(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetOrganization([FromBody] Request<OrganizationRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _organizationService.GetOrganization(request.Payload);
                return Ok(new Response<Organization> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateOrganization([FromBody] Request<UpdateOrganizationDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _organizationService.UpdateOrganization(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteOrganization([FromBody] Request<DeleteOrganizationDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _organizationService.DeleteOrganization(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListOrganization([FromBody] Request<ListOrganizationRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _organizationService.GetListOrganization(request.Payload);
                return Ok(new Response<List<Organization>> { Payload = result });
            });
        }
    }
}
