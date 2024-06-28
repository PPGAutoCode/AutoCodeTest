
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("contact")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateContact([FromBody] Request<CreateContactDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                string result = await _contactService.CreateContact(request.Payload);
                return Ok(new Response<string> { Payload = result });
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
            catch (BusinessException ex)
            {
                return new OkObjectResult(new Response<object>
                {
                    Exception = new ExceptionInfo
                    {
                        Id = Guid.NewGuid().ToString(),
                        Code = ex.Code,
                        Description = ex.Description
                    }
                });
            }
            catch (TechnicalException ex)
            {
                return new OkObjectResult(new Response<object>
                {
                    Exception = new ExceptionInfo
                    {
                        Id = Guid.NewGuid().ToString(),
                        Code = ex.Code,
                        Description = ex.Description
                    }
                });
            }
            catch (Exception)
            {
                return new OkObjectResult(new Response<object>
                {
                    Exception = new ExceptionInfo
                    {
                        Id = Guid.NewGuid().ToString(),
                        Code = "1001",
                        Description = "A technical exception has occurred, please contact your system administrator"
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