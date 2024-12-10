using JobPlatform.Data;
using JobPlatform.DTO.Applicant;
using JobPlatform.DTO.Application;
using JobPlatform.Exceptions;
using JobPlatform.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IAppServices _services;
        private readonly ILogger<ApplicationController> _logger;
        private string? _errors;

        public ApplicationController(IAppServices services, ILogger<ApplicationController> logger)
        {
            _services = services;
            _logger = logger;
        }

        private async Task<ActionResult<IEnumerable<ApplicationViewDTO>>> GetApplicantApplications(long applicantId)
        {
            try
            {
                var result = await _services.ApplicationService.GetApplicantApplications(applicantId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }
        }
        [HttpGet("byapplicant/{applicantId}")]
        public async Task<ActionResult<IEnumerable<ApplicationViewDTO>>> GetApplicantApplicationsInternal(long applicantId)
        {
            return await GetApplicantApplications(applicantId);
        }

        [HttpGet("applicant")]
        public async Task<ActionResult<IEnumerable<ApplicationViewDTO>>> GetApplicantApplicationsApplicant()
        {
            Claim? entityClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if(entityClaim is null || !long.TryParse(entityClaim.Value, out var applicantId))
            {
                return Unauthorized();
            }
            return await GetApplicantApplications(applicantId);
        }

        private async Task<ActionResult<IEnumerable<ApplicationViewDTO>>> GetVacancyApplications(long vacancyId, bool onlyApproved, bool onlyUnchecked)
        {
            try
            {
                var data = await _services.ApplicationService.GetVacancyApplications(vacancyId, onlyUnchecked, onlyApproved);
                return Ok(data);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }
        }

        [HttpGet("byvacancy/{vacancyId}")]
        public async Task<ActionResult<IEnumerable<ApplicationViewDTO>>> GetVacancyApplicationsInternal(long vacancyId, 
            [FromQuery] bool onlyApproved =false, [FromQuery] bool onlyUnchecked = false)
        {
            return await GetVacancyApplications(vacancyId, onlyApproved, onlyUnchecked);
        }

        [HttpGet("business/byvacancy/{vacancyId}")]
        public async Task<ActionResult<IEnumerable<ApplicationViewDTO>>> GetVacancyApplicationsBusiness(long vacancyId,
            [FromQuery] bool onlyApproved = false, [FromQuery] bool onlyUnchecked = false)
        {
            Claim? entityClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if (entityClaim == null || !long.TryParse(entityClaim.Value, out long businessId)) return Unauthorized();
            try
            {
                var vacancy = await _services.VacancyService.GetVacancyById(vacancyId);
                if (vacancy.BusinessId != businessId) return Unauthorized();
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
            return await GetVacancyApplications(vacancyId, onlyApproved, onlyUnchecked);
        }
       
        private  async Task<ActionResult<ApplicationViewExtendedDTO>> GetApplication(long id)
        {
            try
            {
                var result = await _services.ApplicationService.GetApplication(id);
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

        [HttpGet("applicant/{id}")]
        public async Task<ActionResult<ApplicationViewExtendedDTO>> GetApplicationByApplicant(long id)
        {
            Claim? entityClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            try
            {
                var result = await _services.ApplicationService.GetApplication(id);
                if(entityClaim is null || !long.TryParse(entityClaim.Value, out long applicantId)|| result.ApplicantId != applicantId)
                {
                    return Unauthorized();
                }
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

        [HttpGet("business/{id}")]
        public async Task<ActionResult<ApplicationViewExtendedDTO>> GetApplicationsBusiness(long id)
        {
            Claim? entityClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            try
            {
                var result = await _services.ApplicationService.GetApplication(id);
                if( entityClaim is null || !long.TryParse(entityClaim.Value, out long businessId) || result.BusinessId != businessId)
                {
                    return Unauthorized();
                }
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

        [HttpGet("applicant/assess/{vacancyId}")]
        public async Task<ActionResult<ApplicantAssessementDTO>> GetApplicationAssessement(long vacancyId)
        {
            Claim? entityClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if (entityClaim is null || !long.TryParse(entityClaim.Value, out var applicantId))
            {
                return Unauthorized();
            }
            try
            {
                var result = await _services.ApplicationService.AssessApplication(vacancyId, applicantId);
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }

        }

        private async Task<ActionResult<ApplicationViewDTO>> Add(ApplicationInsertDTO insertDTO)
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
                var result = await _services.ApplicationService.AddApplication(insertDTO);
                return Ok(result);
            }
            catch(Exception e)
            {
                _logger?.LogError(e.Message, e);
                return Problem(e.Message);
            }
        }

        [HttpPost("applicant")]
        public async Task<ActionResult<ApplicationViewDTO>> AddForApplicant(ApplicationInsertDTO insertDTO)
        {
            Claim? entityClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if(entityClaim == null || !long.TryParse(entityClaim.Value, out long applicantId))
            {
                return Unauthorized();
            }
            insertDTO.ApplicantId = applicantId;
            return await Add(insertDTO);
        }

        [HttpPost]
        public async Task<ActionResult<ApplicationViewDTO>> AddForInternal(ApplicationInsertDTO insertDTO)
        {
            return await Add(insertDTO);
        }

        [HttpPut("business/checked/{applicationId}")]
        public async Task<ActionResult<ApplicationViewDTO>> ToggleChecked(long applicationId)
        {
            Claim? entityClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if(entityClaim is null ||  !long.TryParse(entityClaim.Value, out long businessId))
            {
                return Unauthorized();
            }
            try
            {
                var result = await _services.ApplicationService.ToggleChecked(applicationId, businessId);
                return Ok(result);
            }
            catch (AccessNotAllowedException)
            {
                return Unauthorized();
            }
            catch(EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut("business/approved/{applicationId}")]
        public async Task<ActionResult<ApplicationViewDTO>> ToggleApproved(long applicationId)
        {
            Claim? entityClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if (entityClaim is null || !long.TryParse(entityClaim.Value, out long businessId))
            {
                return Unauthorized();
            }
            try
            {
                var result = await _services.ApplicationService.ToggleApproved(applicationId, businessId);
                return Ok(result);
            }
            catch (AccessNotAllowedException)
            {
                return Unauthorized();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        private async Task<ActionResult<ApplicationViewDTO>> UpdateText(ApplicationUpdateDTO updateDTO)
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
                var result = await _services.ApplicationService.UpdateApplication(updateDTO);
                return Ok(result);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (AccessNotAllowedException)
            {
                return Unauthorized();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }

        }

        [HttpPut("applicant")]
        public async Task<ActionResult<ApplicationViewDTO>> UpdateTextForApplicant(ApplicationUpdateDTO updateDTO)
        {
            Claim? entityClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if (entityClaim == null || !long.TryParse(entityClaim.Value, out long applicantId))
            {
                return Unauthorized();
            }
            updateDTO.ApplicantId = applicantId;
            return await UpdateText(updateDTO);

        }
        [HttpPut]
        public async Task<ActionResult<ApplicationViewDTO>> UpdateTextInternal(ApplicationUpdateDTO updateDTO)
        {
            return await UpdateText(updateDTO);

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(long id)
        {
            try
            {
                bool result = await _services.ApplicationService.DeleteApplication(id);
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

    }
}
