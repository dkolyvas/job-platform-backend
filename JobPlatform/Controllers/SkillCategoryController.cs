using JobPlatform.DTO.SkillCategory;
using JobPlatform.Exceptions;
using JobPlatform.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillCategoryController : ControllerBase
    {
        private readonly IAppServices _services;
        private readonly ILogger<SkillCategoryController> _logger;
        private string? _errors;

        public SkillCategoryController(IAppServices services, ILogger<SkillCategoryController> logger)
        {
            _services = services;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SkillCategoryViewDTO>>> GetCategories([FromQuery] int? sort)
        {
            if (sort is null) return BadRequest("You must specify the skill sort");
            if (sort < 0 || sort > 2) return BadRequest("Sort id must have a value 0 -2 ");
            try
            {
                var results = await _services.SkillCategoryService.GetSkillCategoriesAsync((int)sort);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SkillCategoryViewDTO>> GetOne(int id)
        {
            try
            {
                var result = await _services.SkillCategoryService.GetSkillCategory(id);
                return Ok(result);
            }
            catch(EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                _logger?.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<SkillCategoryViewDTO>> AddOne(SkillCategoryInsertDTO insertDTO)
        {
            if (!ModelState.IsValid)
            {
                foreach(var value in ModelState.Values)
                {
                    foreach(var error in  value.Errors)
                    {
                        _errors += error.ErrorMessage + " | ";
                    }
                }
                return BadRequest(_errors);
            }
            try
            {
                var result = await _services.SkillCategoryService.Add(insertDTO);
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<SkillCategoryViewDTO>> Update(SkillCategoryUpdateDTO updateDTO)
        {
            if (!ModelState.IsValid)
            {
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        _errors += error.ErrorMessage + " | ";
                    }
                }
                return BadRequest(_errors);
            }
            try
            {
                var result = await _services.SkillCategoryService.Update(updateDTO);
                return Ok(result);
            }
            catch(EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<int>> MergeCategories([FromQuery] int? remaining, int id)
        {
            if(remaining is null) return BadRequest("You must specify the replacing category id");
            try
            {
                int result = await _services.SkillCategoryService.MergeCategories(id, (int)remaining);
                return Ok(result);
            }
            catch(EntityNotFoundException ex)
            {
                return NotFound();
            }
            catch(Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
