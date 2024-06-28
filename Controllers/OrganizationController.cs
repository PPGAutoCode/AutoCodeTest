
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Collections.Generic;
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
        public async Task<IActionResult> CreateOrganization([FromBody] CreateOrganizationDto request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _organizationService.CreateOrganization(request);
                return Ok(new Response<string>(result));
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetOrganization([FromBody] OrganizationDto request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _organizationService.GetOrganization(request);
                return Ok(new Response<Organization>(result));
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateOrganization([FromBody] UpdateOrganizationDto request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _organizationService.UpdateOrganization(request);
                return Ok(new Response<string>(result));
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteOrganization([FromBody] DeleteOrganizationDto request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _organizationService.DeleteOrganization(request);
                return Ok(new Response<bool>(result));
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListOrganization([FromBody] ListOrganizationRequestDto request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _organizationService.GetListOrganization(request);
                return Ok(new Response<List<Organization>>(result));
            });
        }
    }
}
