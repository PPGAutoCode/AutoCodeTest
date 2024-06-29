
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
            var result = await SafeExecutor.ExecuteAsync(async () =>
            {
                var response = await _userQuestionnaireService.CreateUserQuestionnaire(request.Payload);
                return new Response<string> { Payload = response };
            });
            return Ok(result);
        }

        [HttpPost("getList")]
        public async Task<IActionResult> GetListUserQuestionnaire([FromBody] Request<ListUserQuestionnaireRequestDto> request)
        {
            var result = await SafeExecutor.ExecuteAsync(async () =>
            {
                var response = await _userQuestionnaireService.GetListUserQuestionnaire(request.Payload);
                return new Response<List<UserQuestionnaire>> { Payload = response };
            });
            return Ok(result);
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetUserQuestionnaire([FromBody] Request<UserQuestionnaireRequestDto> request)
        {
            var result = await SafeExecutor.ExecuteAsync(async () =>
            {
                var response = await _userQuestionnaireService.GetUserQuestionnaire(request.Payload);
                return new Response<UserQuestionnaire> { Payload = response };
            });
            return Ok(result);
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateUserQuestionnaire([FromBody] Request<UpdateUserQuestionnaireDto> request)
        {
            var result = await SafeExecutor.ExecuteAsync(async () =>
            {
                var response = await _userQuestionnaireService.UpdateUserQuestionnaire(request.Payload);
                return new Response<string> { Payload = response };
            });
            return Ok(result);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUserQuestionnaire([FromBody] Request<DeleteUserQuestionnaireDto> request)
        {
            var result = await SafeExecutor.ExecuteAsync(async () =>
            {
                var response = await _userQuestionnaireService.DeleteUserQuestionnaire(request.Payload);
                return new Response<bool> { Payload = response };
            });
            return Ok(result);
        }
    }
}
