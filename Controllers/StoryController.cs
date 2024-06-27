
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoryController : ControllerBase
    {
        private readonly IStoryService _storyService;

        public StoryController(IStoryService storyService)
        {
            _storyService = storyService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateStory([FromBody] Request<CreateStoryDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _storyService.CreateStory(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetStory([FromBody] Request<RequestStoryDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _storyService.GetStory(request.Payload);
                return Ok(new Response<Story> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateStory([FromBody] Request<UpdateStoryDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _storyService.UpdateStory(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteStory([FromBody] Request<DeleteStoryDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _storyService.DeleteStory(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListStory([FromBody] Request<ListStoryRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _storyService.GetListStory(request.Payload);
                return Ok(new Response<List<Story>> { Payload = result });
            });
        }
    }
}
