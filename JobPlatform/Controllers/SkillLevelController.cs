using JobPlatform.DTO.Skill_Level;
using JobPlatform.Exceptions;
using JobPlatform.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillLevelController : ControllerBase
    {
        private IAppServices _services;
        private ILogger<SkillLevelController> _logger;

        public SkillLevelController(IAppServices services, ILogger<SkillLevelController> logger)
        {
            _services = services;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SkillLevelViewDTO>>> GetSkillLevels([FromQuery] int? sort,
            [FromQuery] int? category, [FromQuery] long? subcategory)
        {
            try
            {
                IEnumerable<SkillLevelViewDTO> levels = new List<SkillLevelViewDTO>();
                if(sort != null)
                {
                    levels = await _services.SkillLevelService.FindSkillLevelsForSort((int)sort);
                }
                else if(category != null)
                {
                    levels = await _services.SkillLevelService.FindSkillLevelsForCategory((int)category);
                }
                else if(subcategory != null)
                {
                    levels = await _services.SkillLevelService.FindSkillLevelsForSubcategory((long)subcategory);
                }
                return Ok(levels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SkillLevelViewDTO>> GetById(int id)
        {
            try
            {
                var result = await _services.SkillLevelService.FindById(id);
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

        [HttpPost]
        public async Task<ActionResult<SkillLevelViewDTO>> AddLevel(SkillLevelInsertDTO insertDTO)
        {
            if (!ModelState.IsValid)
            {
                string errors = "";
                foreach(var value in ModelState.Values)
                {
                    foreach(var error in value.Errors)
                    {
                        errors += error.ErrorMessage + " | ";
                    }
                }
                return BadRequest(errors);
            }
            try
            {
                var result = await _services.SkillLevelService.AddLevel(insertDTO);
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<SkillLevelViewDTO>> Update(SkillLevelUpdateDTO updateDTO)
        {

            if (!ModelState.IsValid)
            {
                string errors = "";
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors += error.ErrorMessage + " | ";
                    }
                }
                return BadRequest(errors);
            }
            try
            {
                var result = await _services.SkillLevelService.UpdateLevel(updateDTO);
                return Ok(result);
            }
            catch(EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Problem(ex.Message) ;
            }

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            try
            {
                var result = await _services.SkillLevelService.Delete(id);
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
    }
}
