using Microsoft.AspNetCore.Mvc;
using warehouseapp.Interfaces;

namespace warehouseapp.Controllers
{
    [ApiController]
    [Route("api/inventory/transactions")]
    public class InventoryTransactionsController : ControllerBase
    {
        private readonly IInventoryTransactionService _service;

        public InventoryTransactionsController(IInventoryTransactionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }
    }
}
