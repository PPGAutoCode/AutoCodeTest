
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
    public class AllowedGrantTypeController : ControllerBase
    {
        private readonly IAllowedGrantTypeService _allowedGrantTypeService;

        public AllowedGrantTypeController(IAllowedGrantTypeService allowedGrantTypeService)
        {
            _allowedGrantTypeService = allowedGrantTypeService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAllowedGrantType([FromBody] Request<CreateAllowedGrantTypeDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _allowedGrantTypeService.CreateAllowedGrantType(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetAllowedGrantType([FromBody] Request<AllowedGrantTypeRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _allowedGrantTypeService.GetAllowedGrantType(request.Payload);
                return Ok(new Response<AllowedGrantType> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateAllowedGrantType([FromBody] Request<UpdateAllowedGrantTypeDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _allowedGrantTypeService.UpdateAllowedGrantType(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteAllowedGrantType([FromBody] Request<DeleteAllowedGrantTypeDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _allowedGrantTypeService.DeleteAllowedGrantType(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListAllowedGrantType([FromBody] Request<ListAllowedGrantTypeRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _allowedGrantTypeService.GetListAllowedGrantType(request.Payload);
                return Ok(new Response<List<AllowedGrantType>> { Payload = result });
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
                var exceptionResponse = new Response<object>
                {
                    Exception = new ExceptionInfo
                    {
                        Id = Guid.NewGuid().ToString(),
                        Code = ex is BusinessException || ex is TechnicalException ? ex.Code : "1001",
                        Description = ex is BusinessException || ex is TechnicalException ? ex.Description : "A technical exception has occurred, please contact your system administrator"
                    }
                };
                return new OkObjectResult(exceptionResponse);
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
