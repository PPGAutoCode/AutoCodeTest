It appears that the error message indicates a missing semicolon at line 90, column 115 in the `ImageController.cs` file. Let's review the code around that line to identify and fix the issue.

Here's the corrected code with the missing semicolon added:

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

I have added the missing semicolon at the specified location. This should resolve the compilation error without altering the logic of the code.