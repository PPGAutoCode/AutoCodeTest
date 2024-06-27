using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productService.CreateProduct(productDto);
                return Ok(result);
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(string id)
        {
            var requestDto = new ProductRequestDto { Id = id };
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productService.GetProduct(requestDto);
                return Ok(result);
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] UpdateProductDto productDto)
        {
            productDto.Id = id;
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productService.UpdateProduct(productDto);
                return Ok(result);
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var requestDto = new DeleteProductDto { Id = id };
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productService.DeleteProduct(requestDto);
                return Ok(result);
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetListProduct([FromQuery] ListProductRequestDto requestDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productService.GetListProduct(requestDto);
                return Ok(result);
            });
        }
    }
}
