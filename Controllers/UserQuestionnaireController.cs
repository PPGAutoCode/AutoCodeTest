
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserQuestionnaireController : ControllerBase
    {
        private readonly IUserQuestionnaireService _userQuestionnaireService;

        public UserQuestionnaireController(IUserQuestionnaireService userQuestionnaireService)
        {
            _userQuestionnaireService = userQuestionnaireService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUserQuestionnaire([FromBody] Request<CreateUserQuestionnaireDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userQuestionnaireService.CreateUserQuestionnaire(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("getList")]
        public async Task<IActionResult> GetUserQuestionnairesList([FromBody] Request<ListUserQuestionnaireRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userQuestionnaireService.GetUserQuestionnairesList(request.Payload);
                return Ok(new Response<List<UserQuestionnaire>> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetUserQuestionnaire([FromBody] Request<UserQuestionnaireRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userQuestionnaireService.GetUserQuestionnaire(request.Payload);
                return Ok(new Response<UserQuestionnaire> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateUserQuestionnaire([FromBody] Request<UpdateUserQuestionnaireDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userQuestionnaireService.UpdateUserQuestionnaire(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUserQuestionnaire([FromBody] Request<DeleteUserQuestionnaireDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userQuestionnaireService.DeleteUserQuestionnaire(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }
    }
}
