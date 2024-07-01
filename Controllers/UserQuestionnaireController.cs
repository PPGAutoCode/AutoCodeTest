
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("userQuestionnaire")]
    public class UserQuestionnaireController : ControllerBase
    {
        private readonly IUserQuestionnaireService _userQuestionnaireService;

        public UserQuestionnaireController(IUserQuestionnaireService userQuestionnaireService)
        {
            _userQuestionnaireService = userQuestionnaireService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUserQuestionnaire([FromBody] CreateUserQuestionnaireDto request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userQuestionnaireService.CreateUserQuestionnaire(request);
                return Ok(new Response<string>(result));
            });
        }

        [HttpPost("getList")]
        public async Task<IActionResult> GetListUserQuestionnaire([FromBody] ListUserQuestionnaireRequestDto request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userQuestionnaireService.GetListUserQuestionnaire(request);
                return Ok(result);
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetUserQuestionnaire([FromBody] UserQuestionnaireRequestDto request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userQuestionnaireService.GetUserQuestionnaire(request);
                return Ok(new Response<UserQuestionnaire>(result));
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateUserQuestionnaire([FromBody] UpdateUserQuestionnaireDto request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userQuestionnaireService.UpdateUserQuestionnaire(request);
                return Ok(new Response<UserQuestionnaire>(result));
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUserQuestionnaire([FromBody] DeleteUserQuestionnaireDto request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userQuestionnaireService.DeleteUserQuestionnaire(request);
                return Ok(new Response<bool>(result));
            });
        }
    }
}
