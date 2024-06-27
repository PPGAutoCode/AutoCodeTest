
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System;
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
        public async Task<IActionResult> CreateImage([FromBody] Request<CreateImageDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _imageService.CreateImage(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetImage([FromBody] Request<ImageRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _imageService.GetImage(request.Payload);
                return Ok(new Response<Image> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateImage([FromBody] Request<UpdateImageDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _imageService.UpdateImage(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteImage([FromBody] Request<DeleteImageDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _imageService.DeleteImage(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListImage([FromBody] Request<ListImageRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _imageService.GetListImage(request.Payload);
                return Ok(new Response<List<Image>> { Payload = result });
            });
        }
    }

    public static class SafeExecutor
    {
        public static async Task<IActionResult> ExecuteAsync(Func<Task<IActionResult>> action)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new Response<object>
                {
                    Exception = new ExceptionInfo
                    {
                        Id = Guid.NewGuid().ToString(),
                        Code = ex is BusinessException || ex is TechnicalException ? ex.GetType().Name : "1001",
                        Description = ex is BusinessException || ex is TechnicalException ? ex.Message : "A technical exception has occurred, please contact your system administrator"
                    }
                });
            }
        }
    }

    public class Response<T>
    {
        public T Payload { get; set; }
        public ExceptionInfo Exception { get; set; }
    }

    public class ExceptionInfo
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class Request<T>
    {
        public Header Header { get; set; }
        public T Payload { get; set; }
    }

    public class Header
    {
        public string ID { get; set; }
        public string Application { get; set; }
        public string Bank { get; set; }
        public string UserId { get; set; }
    }

    public class BusinessException : Exception
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class TechnicalException : Exception
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
