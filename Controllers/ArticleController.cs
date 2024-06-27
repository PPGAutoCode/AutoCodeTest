
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Collections.Generic;
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
        public async Task<IActionResult> CreateArticle([FromBody] CreateArticleDto createArticleDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _articleService.CreateArticle(createArticleDto);
                return Ok(result);
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetArticle([FromBody] ArticleRequestDto articleRequestDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _articleService.GetArticle(articleRequestDto);
                return Ok(result);
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateArticle([FromBody] UpdateArticleDto updateArticleDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _articleService.UpdateArticle(updateArticleDto);
                return Ok(result);
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteArticle([FromBody] DeleteArticleDto deleteArticleDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _articleService.DeleteArticle(deleteArticleDto);
                return Ok(result);
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListArticle([FromBody] ListArticleRequestDto listArticleRequestDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _articleService.GetListArticle(listArticleRequestDto);
                return Ok(result);
            });
        }
    }
}
