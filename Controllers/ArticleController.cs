
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateArticle([FromBody] Request<CreateArticleDto> request)
        {
            return Ok(await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _articleService.CreateArticle(request.Payload);
                return new Response<string> { Payload = result };
            }));
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetArticle([FromBody] Request<ArticleRequestDto> request)
        {
            return Ok(await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _articleService.GetArticle(request.Payload);
                return new Response<Article> { Payload = result };
            }));
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateArticle([FromBody] Request<UpdateArticleDto> request)
        {
            return Ok(await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _articleService.UpdateArticle(request.Payload);
                return new Response<string> { Payload = result };
            }));
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteArticle([FromBody] Request<DeleteArticleDto> request)
        {
            return Ok(await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _articleService.DeleteArticle(request.Payload);
                return new Response<bool> { Payload = result };
            }));
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListArticle([FromBody] Request<ListArticleRequestDto> request)
        {
            return Ok(await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _articleService.GetListArticle(request.Payload);
                return new Response<List<Article>> { Payload = result };
            }));
        }
    }
}
