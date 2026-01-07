using Microsoft.AspNetCore.Mvc;
using warehouseapp.Interfaces;
using warehouseapp.ViewModels.Product;
using warehouseapp.Exceptions;

namespace warehouseapp.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await _service.GetAllProductsAsync());
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                return Ok(await _service.GetProductByIdAsync(id));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id:int}/edit")]
        public async Task<IActionResult> GetForEdit(int id)
        {
            try
            {
                return Ok(await _service.GetProductForEditAsync(id));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductRequestViewModel model)
        {
            try
            {
                return Ok(await _service.CreateProductAsync(model));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] ProductRequestViewModel model)
        {
            try
            {
                return Ok(await _service.UpdateProductAsync(id, model));
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
                await _service.DeleteProductAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter( [FromQuery] string? search,[FromQuery] int? categoryId,[FromQuery] bool? hasExpiration)
        {
            try
            {
                return Ok(await _service.FilterAsync(search, categoryId, hasExpiration));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("select")]
        public async Task<IActionResult> GetForSelect()
        {
            try
            {
                return Ok(await _service.GetForSelectAsync());
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}