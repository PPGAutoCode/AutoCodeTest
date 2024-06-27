
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
                return Ok(new Response<string>(result));
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetImage([FromBody] ImageRequestDto imageRequestDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _imageService.GetImage(imageRequestDto);
                return Ok(new Response<Image>(result));
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateImage([FromBody] UpdateImageDto updateImageDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _imageService.UpdateImage(updateImageDto);
                return Ok(new Response<string>(result));
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteImage([FromBody] DeleteImageDto deleteImageDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _imageService.DeleteImage(deleteImageDto);
                return Ok(new Response<bool>(result));
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListImage([FromBody] ListImageRequestDto listImageRequestDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _imageService.GetListImage(listImageRequestDto);
                return Ok(new Response<List<Image>>(result));
            });
        }
    }
}
