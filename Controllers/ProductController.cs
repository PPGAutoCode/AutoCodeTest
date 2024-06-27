
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
                var result = await _productService.CreateProduct(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetProduct([FromBody] Request<ProductRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productService.GetProduct(request.Payload);
                return Ok(new Response<Product> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateProduct([FromBody] Request<UpdateProductDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productService.UpdateProduct(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteProduct([FromBody] Request<DeleteProductDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productService.DeleteProduct(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListProduct([FromBody] Request<ListProductRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productService.GetListProduct(request.Payload);
                return Ok(new Response<List<Product>> { Payload = result });
            });
        }
    }

    public class Request<T>
    {
