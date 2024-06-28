
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
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSubscription([FromBody] Request<CreateSubscriptionDTO> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _subscriptionService.CreateSubscription(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetSubscription([FromBody] Request<RequestSubscription> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _subscriptionService.GetSubscription(request.Payload);
                return Ok(new Response<Subscription> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateSubscription([FromBody] Request<UpdateSubscriptionDTO> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _subscriptionService.UpdateSubscription(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteSubscription([FromBody] Request<DeleteSubscriptionDTO> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _subscriptionService.DeleteSubscription(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListSubscription([FromBody] Request<ListSubscriptionRequestDTO> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _subscriptionService.GetListSubscription(request.Payload);
                return Ok(new Response<List<Subscription>> { Payload = result });
            });
        }
    }
}
