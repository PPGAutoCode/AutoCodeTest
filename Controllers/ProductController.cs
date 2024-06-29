
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("product")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromBody] Request<CreateProductDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                string result = await _productService.CreateProduct(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }
    }
}
