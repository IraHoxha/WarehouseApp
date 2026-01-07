using Microsoft.AspNetCore.Mvc;
using warehouseapp.Exceptions;
using warehouseapp.Interfaces;
using warehouseapp.ViewModels.Tag;

namespace warehouseapp.Controllers
{
    [ApiController]
    [Route("api/tag")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _tagService.GetAllAsync());
        }

        [HttpGet("{id:int}/values")]
        public async Task<IActionResult> GetValues(int id)
        {
            try
            {
                return Ok(await _tagService.GetValuesAsync(id));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TagRequestViewModel model)
        {
            try
            {
                return Ok(await _tagService.CreateAsync(model));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            try
            {
                await _tagService.DeleteIfUnusedAsync(id);
                return Ok(true);
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

        [HttpDelete("value/{id:int}")]
        public async Task<IActionResult> DeleteValue(int id)
        {
            try
            {
                await _tagService.DeleteTagValueIfUnusedAsync(id);
                return Ok(true);
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

        [HttpPost("cleanup")]
        public async Task<IActionResult> Cleanup()
        {
            await _tagService.CleanupAsync();
            return Ok(true);
        }
    }
}
