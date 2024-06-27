
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] Request<CreateUserDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userService.CreateUser(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetUser([FromBody] Request<UserRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userService.GetUser(request.Payload);
                return Ok(new Response<User> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateUser([FromBody] Request<UpdateUserDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userService.UpdateUser(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUser([FromBody] Request<DeleteUserDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userService.DeleteUser(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListUsers([FromBody] Request<ListUserRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userService.GetListUsers(request.Payload);
                return Ok(new Response<List<User>> { Payload = result });
            });
        }
    }
}
