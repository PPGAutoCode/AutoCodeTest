
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ProjectName.Controllers
{
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
                        Code = ex is BusinessException || ex is TechnicalException ? ex.GetType().Name : "1001",
                        Description = ex is BusinessException || ex is TechnicalException ? ex.Message : "A technical exception has occurred, please contact your system administrator"
                    }
                };
                return new OkObjectResult(exceptionResponse);
            }
        }
    }
}
