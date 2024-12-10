using JobPlatform.DTO.Region;
using JobPlatform.Exceptions;
using JobPlatform.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionController : ControllerBase
    {
        private readonly IAppServices _services;
        private readonly ILogger<RegionController> _logger;

        public RegionController(IAppServices services, ILogger<RegionController> logger)
        {
            _services = services;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegionViewDTO>>> GetAll()
        {
            try
            {
                var result = await _services.RegionService.GetAll();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RegionViewDTO>> GetOne(int id)
        {
            try
            {
                var result = await _services.RegionService.GetById(id);
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

        [HttpPost]
        public async Task<ActionResult<RegionViewDTO>> Add(RegionInsertDTO insertDTO)
        {
            if(!ModelState.IsValid)
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
                var result = await _services.RegionService.AddOne(insertDTO);
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<RegionViewDTO>> Update(RegionUpdateDTO updateDTO)
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
                var result = await _services.RegionService.UpdateOne(updateDTO);
                return Ok(result);
            }
            catch (EntityNotFoundException ex)
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
        public async Task<ActionResult<bool>> Delete(int id)
        {
            try
            {
                bool result = await _services.RegionService.DeleteById(id);
                return Ok(result);
            }
            catch (EntityNotFoundException ex)
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
