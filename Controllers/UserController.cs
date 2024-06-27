
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] Request<CreateUserRequestDTO> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userService.CreateUser(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetUser([FromBody] Request<UserRequestDTO> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userService.GetUser(request.Payload);
                return Ok(new Response<User> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateUser([FromBody] Request<UpdateUserRequestDTO> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userService.UpdateUser(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUser([FromBody] Request<DeleteUserRequestDTO> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userService.DeleteUser(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListUser([FromBody] Request<ListUserRequestDTO> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userService.GetListUser(request.Payload);
                return Ok(new Response<List<User>> { Payload = result });
            });
        }
    }
}
