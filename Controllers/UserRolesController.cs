
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserRolesController : ControllerBase
    {
        private readonly IUserRolesService _userRolesService;

        public UserRolesController(IUserRolesService userRolesService)
        {
            _userRolesService = userRolesService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUserRole([FromBody] Request<CreateUserRoleDTO> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userRolesService.CreateUserRole(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("getList")]
        public async Task<IActionResult> GetListUserRoles([FromBody] Request<UserRolesRequestDTO> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userRolesService.GetListUserRoles(request.Payload);
                return Ok(result);
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetUserRole([FromBody] Request<UserRoleRequestDTO> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userRolesService.GetUserRole(request.Payload);
                return Ok(new Response<UserRoles> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateUserRole([FromBody] Request<UpdateUserRoleDTO> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userRolesService.UpdateUserRole(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUserRole([FromBody] Request<DeleteUserRoleDTO> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userRolesService.DeleteUserRole(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }
    }
}
