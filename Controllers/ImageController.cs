It appears that the error message you provided indicates an issue with the `SafeExecutor.ExecuteAsync` method. The error message `CS1733: Expected expression` suggests that there might be a missing expression or a syntax error within the lambda expression passed to `SafeExecutor.ExecuteAsync`.

Let's review and correct the code for the `ImageController` class to ensure it compiles without errors. Here is the corrected code:

```csharp
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateImage([FromBody] CreateImageDto createImageDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _imageService.CreateImage(createImageDto);
                return Ok(result);
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetImage([FromBody] ImageRequestDto imageRequestDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _imageService.GetImage(imageRequestDto);
                return Ok(result);
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateImage([FromBody] UpdateImageDto updateImageDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _imageService.UpdateImage(updateImageDto);
                return Ok(result);
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteImage([FromBody] DeleteImageDto deleteImageDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _imageService.DeleteImage(deleteImageDto);
                return Ok(result);
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListImage([FromBody] ListImageRequestDto listImageRequestDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _imageService.GetListImage(listImageRequestDto);
                return Ok(result);
            });
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class BlogTagController : ControllerBase
    {
        private readonly IBlogTagService _blogTagService;

        public BlogTagController(IBlogTagService blogTagService)
        {
            _blogTagService = blogTagService;
        }
    }
}
```

In this corrected code, I have ensured that all lambda expressions passed to `SafeExecutor.ExecuteAsync` are correctly formed and that all classes are properly closed with the necessary braces. This should resolve the compilation error and maintain the logic of the code.