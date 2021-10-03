using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SimpleFinanceTestTask.ApplicationCore.Interfaces.Services;

namespace SimpleFinanceTestTask.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("/obj")]
    public class PoolObjectController : ControllerBase
    {
        private readonly IPoolObjectAppService _poolObjectAppService;

        public PoolObjectController(IPoolObjectAppService poolObjectAppService)
        {
            _poolObjectAppService = poolObjectAppService;
        }

        [HttpGet("acquire/{itemId}")]
        public async Task<IActionResult> GetItemAsync(int itemId)
        {
            var serviceResponse = await _poolObjectAppService.TryGetItemAsync(itemId);

            if (serviceResponse.response is false)
                return Forbid();

            return Ok(serviceResponse.item);
        }

        [HttpPost("return/{itemId}")]
        public async Task<IActionResult> ReturnItemAsync(int itemId)
        {
            var serviceResponse = await _poolObjectAppService.TryReleaseItemAsync(itemId);

            if (serviceResponse is false)
                return BadRequest();

            return Ok(new object());
        }
    }
}