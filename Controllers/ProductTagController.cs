using Microsoft.AspNetCore.Mvc;
using warehouseapp.Exceptions;
using warehouseapp.Interfaces;
using warehouseapp.ViewModels.Tag;

namespace warehouseapp.Controllers
{
    [ApiController]
    [Route("api/product-tag")]
    public class ProductTagController : ControllerBase
    {
        private readonly IProductTagService _service;

        public ProductTagController(IProductTagService service)
        {
            _service = service;
        }

        [HttpGet("{productId:int}")]
        public async Task<IActionResult> Get(int productId)
        {
            try
            {
                var result = await _service.GetByProductIdAsync(productId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ProductTagRequestViewModel model)
        {
            try
            {
                var result = await _service.AddAsync(model);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok(true);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
