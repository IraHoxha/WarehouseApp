using Microsoft.AspNetCore.Mvc;
using warehouseapp.Interfaces;
using warehouseapp.ViewModels.Order;

namespace warehouseapp.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrderController(IOrderService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderRequestViewModel model)
        {
            var result = await _service.CreateAsync(model);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _service.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _service.GetByIdAsync(id);
            return Ok(order);
        }

        [HttpPost("{id:int}/process")]
        public async Task<IActionResult> Process(int id)
        {
            await _service.ProcessAsync(id);
            return NoContent();
        }

        [HttpPost("{id:int}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            await _service.CompleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            await _service.CancelAsync(id);
            return NoContent();
        }

        [HttpPost("{id:int}/return")]
        public async Task<IActionResult> Return(
            int id,
            OrderReturnRequestViewModel model)
        {
            await _service.ReturnAsync(id, model);
            return NoContent();
        }
    }
}
