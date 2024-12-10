using JobPlatform.DTO.SubscriptionType;
using JobPlatform.Exceptions;
using JobPlatform.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionTypeController : ControllerBase
    {
        private readonly IAppServices _services;
        private readonly ILogger<SubscriptionTypeController> _logger;

        public SubscriptionTypeController(IAppServices services, ILogger<SubscriptionTypeController> logger)
        {
            _services = services;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubscriptionTypeViewDTO>>> GetAll()
        {
            try
            {
                var result = await _services.SubscriptionTypeService.GetAll();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SubscriptionTypeViewDTO>> GetById(int id)
        {
            try
            {
                var result = await _services.SubscriptionTypeService.GetById(id);
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
        public async Task<ActionResult<SubscriptionTypeViewDTO>> Add(SubscriptionTypeInsertDTO dto)
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
                var result = await _services.SubscriptionTypeService.AddOne(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<SubscriptionTypeViewDTO>> Update(SubscriptionTypeUpdateDTO dto)
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
                var result = await _services.SubscriptionTypeService.UpdateOne(dto);
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
                var result = await _services.SubscriptionTypeService.DeleteById(id);
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
