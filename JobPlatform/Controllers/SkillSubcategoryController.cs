using JobPlatform.DTO.SkillSubacategory;
using JobPlatform.Exceptions;
using JobPlatform.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillSubcategoryController : ControllerBase
    {
        private readonly IAppServices _services;
        private readonly ILogger<SkillSubcategoryController> _logger;
        private string? _errors;

        public SkillSubcategoryController(IAppServices services, ILogger<SkillSubcategoryController> logger)
        {
            _services = services;
            _logger = logger;
        }

        [HttpGet("category/{catId}")]
        public async Task<ActionResult<IEnumerable<SkillSubcategoryViewDTO>>> GetByCategory(int catId)
        {
            try
            {
                var results = await _services.SkillSubcategoryService.GetByCategory(catId);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }
        }

        [HttpGet("unchecked")]
        public async Task<ActionResult<IEnumerable<SkillSubcategoryViewDTO>>> GetUnchecked()
        {
            try
            {
                var results = await _services.SkillSubcategoryService.GetUnchecked();
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SkillSubcategoryViewDTO>> GetById(long id)
        {
            try
            {
                var result = await _services.SkillSubcategoryService.GetById(id);
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
        public async Task<ActionResult<SkillSubcategoryViewDTO>> AddOne(SkillSubcategoryInsertDTO insertDTO)
        {
            if (!ModelState.IsValid)
            {
                foreach(var value in ModelState.Values)
                {
                    foreach(var error in value.Errors)
                    {
                        _errors += error.ErrorMessage + " | ";
                    }
                }
                return BadRequest(_errors);
            }
            try
            {
                var result = await _services.SkillSubcategoryService.Add(insertDTO);
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Problem(ex.ToString());
            }
        }

        [HttpPut]
        public async Task<ActionResult<SkillSubcategoryViewDTO>> Update(SkillSubcategoryUpdateDTO updateDTO)
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
                var result = await  _services.SkillSubcategoryService.Update(updateDTO);
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
        [HttpPut("checked/{id}")]
        public async  Task<ActionResult<SkillSubcategoryViewDTO>> MarkAsChecked(long id)
        {
            try
            {
                var result = await _services.SkillSubcategoryService.MarkAsChecked(id);
                return Ok(result);
            }
            catch(EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<int>> MergeSubcateogries(long id, [FromQuery] long? remaining) 
        {
            if (remaining is null) return BadRequest("you must provide the replacing subcategory id");
            try
            {
                int result = await _services.SkillSubcategoryService.MergeSubcategories(id, (long)remaining);
                return Ok(result);
            }
            catch(EntityNotFoundException ex)
            {
                return NotFound(ex.Message) ;

            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message , ex);
                return Problem(ex.Message );
            }
        }



    }
}
