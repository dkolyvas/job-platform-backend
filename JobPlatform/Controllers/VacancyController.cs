using JobPlatform.DTO.SearchCriteria;
using JobPlatform.DTO.Vacancy;
using JobPlatform.Exceptions;
using JobPlatform.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacancyController : ControllerBase
    {
        private readonly IAppServices _services;
        private readonly ILogger<VacancyController> _logger;
        private string? _errors;

        public VacancyController(IAppServices services, ILogger<VacancyController> logger)
        {
            _services = services;
            _logger = logger;
        }

        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<VacancyViewDTO>>> FindVacancies(VacancySearchCriteria searchCriteria)
        {
            try
            {
                var results = await _services.VacancyService.FindVacancies(searchCriteria);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }
        }
        [HttpGet("business")]
        public async Task<ActionResult<IEnumerable<VacancyViewDTO>>> FindConnectedBusinessVacancies()
        {
            Claim? entityClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if(entityClaim == null || !long.TryParse(entityClaim.Value, out long businessId)) 
            {
                return Unauthorized();
            }
            try
            {
                var results = await _services.VacancyService.FindBusinessVacancies(businessId);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }
        }


        [HttpGet("bybusiness/{businessId}")]
        public async Task<ActionResult<IEnumerable<VacancyViewDTO>>> FindBusinessVacancies(long businessId)
        {
            try
            {
                var results = await _services.VacancyService.FindBusinessVacancies(businessId);
                return Ok(results);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VacancyViewExtendedDTO>> GetVacancy(long id)
        {
            try
            {
                var result = await _services.VacancyService.GetVacancyById(id);
                return Ok(result);
            }
            catch(EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }
        }

        private async Task<ActionResult<VacancyViewExtendedDTO>> Add(VacancyInsertDTO insertDTO)
        {
            if (!ModelState.IsValid)
            {
                foreach(var val in ModelState.Values)
                {
                    foreach(var error in val.Errors)
                    {
                        _errors += error.ErrorMessage + " | ";
                    }
                }
                return BadRequest(_errors);
            }
            try
            {
                var result = await _services.VacancyService.AddVacancy(insertDTO);
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                if(ex is AllowanceExceededException || ex is NoActiveSubscriptionException)
                {
                    return BadRequest(ex.Message);
                }
                else
                {
                    return Problem(ex.Message);
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult<VacancyViewExtendedDTO>> AddVacancy(VacancyInsertDTO insertDTO)
        {
            return await Add(insertDTO);
        }

        [HttpPost("business")]
        public async Task<ActionResult<VacancyViewExtendedDTO>> AddVacancyBusiness(VacancyInsertDTO insertDTO)
        {
            Claim? entityClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if(entityClaim == null || ! long.TryParse(entityClaim.Value, out long businessId))
            {
                return Unauthorized();
            }
            else
            {
                insertDTO.BusinessId = businessId;
                return await Add(insertDTO);
            }
        }

        private async Task<ActionResult<VacancyViewExtendedDTO>> Update(VacancyUpdateDTO updateDTO)
        {
            if (!ModelState.IsValid)
            {
                foreach (var val in ModelState.Values)
                {
                    foreach (var error in val.Errors)
                    {
                        _errors += error.ErrorMessage + " | ";
                    }
                }
                return BadRequest(_errors);
            }
            try
            {
                var result = await _services.VacancyService.UpdateVacancy(updateDTO);
                return Ok(result);
            }
            catch(AccessNotAllowedException)
            {
                return Unauthorized();
            }
            catch(EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }

        }

        [HttpPut("business")]
        public async Task<ActionResult<VacancyViewExtendedDTO>> UpdateBusiness(VacancyUpdateDTO updateDTO)
        {
            Claim? entityClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if (entityClaim == null || !long.TryParse(entityClaim.Value, out long businessId))
            {
                return Unauthorized();
            }
            updateDTO.BusinessId = businessId;
            return await Update(updateDTO);

        }

        [HttpPut]
        public async Task<ActionResult<VacancyViewExtendedDTO>> UpdateInternal(VacancyUpdateDTO updateDTO)
        {
            return await Update(updateDTO);
        }

        [HttpPut("disactivate/{id}")]
        public async Task<ActionResult<VacancyViewDTO>> DisactivateInternal(long id)
        {
            try
            {
                var result = await _services.VacancyService.DisactivateVacancy(id);
                return Ok(result);
            }
            catch(EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }
        }

        [HttpPut("business/disactivate/{id}")]
        public async Task<ActionResult<VacancyViewDTO>> DisactivateForBusiness(long id)
        {
            Claim? entityClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if(entityClaim == null || !long.TryParse(entityClaim.Value, out long businessId)) 
            {
                return Unauthorized();
            }
            try
            {
                var result = await _services.VacancyService.DisactivateVacancy(id, businessId);
                return Ok(result);
            }
            catch(AccessNotAllowedException)
            {
                return Unauthorized();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var result = await _services.VacancyService.DeleteVacancyById(id);
                return Ok(result);
            }
            catch(EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }
        }




    }
}
