using JobPlatform.Data;
using JobPlatform.DTO.Applicant;
using JobPlatform.Exceptions;
using JobPlatform.Services;
using JobPlatform.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicantController : ControllerBase
    {
        private readonly IAppServices _services;
        private readonly ILogger<ApplicantController> _logger;
        private readonly IWebHostEnvironment _environment;
        private string? _errors;

        public ApplicantController(IAppServices services, ILogger<ApplicantController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _services = services;
            _logger = logger;
            _environment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicantViewDTO>>> GetApplicants([FromQuery] string? firstname,
            [FromQuery] string? lastname, [FromQuery] string? email)
        {
            IEnumerable<ApplicantViewDTO> results;
            try
            {
                if (firstname != null || lastname != null || email != null)
                {
                    results = await _services.ApplicantService.FindApplicants(firstname, lastname, email);
                    return Ok(results);
                }
                else
                {
                    results = await _services.ApplicantService.GetApplicantsAsync();
                    return Ok(results);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicantViewExtendedDTO>> GetById(long id)
        {
            try
            {
                var applicant = await _services.ApplicantService.GetApplicantById(id);
                return Ok(applicant);
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
        [HttpGet("applicant")]
        public async Task<ActionResult<ApplicantViewExtendedDTO>> GetCurrentApplicant()
        {
            Claim? entityIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if (entityIdClaim == null || !long.TryParse(entityIdClaim.Value, out long id)) {
                return Unauthorized();
            }
            try
            {
                var applicant = await _services.ApplicantService.GetApplicantById(id);
                return Ok(applicant);
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
        [HttpGet("business/{id}")]
        public async Task<ActionResult<ApplicantViewExtendedDTO>> GetApplicantForBusiness(long id)
        {
            Claim? entityClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if (entityClaim is null || !long.TryParse(entityClaim.Value, out long businessId)){
                return Unauthorized();
            }
            try
            {
                var applicant = await _services.ApplicantService.GetApplicantForBusiness(id, businessId);
                return Ok(applicant);
            }
            catch(AccessNotAllowedException ex)
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

        private ActionResult GetApplicantCv(string filename)
        {
            string path = Path.Combine(_environment.WebRootPath, "Pdfs", filename);
            if (!System.IO.File.Exists(path)) return NotFound();
            try
            {
                var stream = new MemoryStream(System.IO.File.ReadAllBytes(path));
                stream.Position = 0;
                var file = File(stream, "application/pdf", $"cv.pdf");
                return Ok(file);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);

            }
        }

        [HttpGet("cv/{id}")]
        public async Task<ActionResult> GetApplicantCvById(long id)
        {
            try
            {
                var applicant = await _services.ApplicantService.GetApplicantById(id);
                if (applicant.Cv == null) return NotFound();
                return GetApplicantCv(applicant.Cv);

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

        [HttpGet("applicant/cv")]
        public async Task<ActionResult> GetCurrentUserCv()
        {
            Claim? entityIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if (entityIdClaim == null || !long.TryParse(entityIdClaim.Value, out long id)) {
                return Unauthorized();
            }
            try
            {
                var applicant = await _services.ApplicantService.GetApplicantById(id);
                if (applicant.Cv is null) return NotFound();
                return GetApplicantCv(applicant.Cv);
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


        [HttpGet("business/cv/{applicantId}")]
        public async Task<ActionResult> GetCvForBusiness(long applicantId)
        {
            Claim? entityClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if (entityClaim is null || !long.TryParse(entityClaim.Value, out long businessId))
            {
                return Unauthorized();
            }
            try
            {
                var applicant = await _services.ApplicantService.GetApplicantForBusiness(applicantId, businessId);
                if (applicant.Cv is null) return NotFound();
                return GetApplicantCv(applicant.Cv);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(AccessNotAllowedException ex)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }

        }

        private async Task<ActionResult<ApplicantViewExtendedDTO>> Insert(ApplicantInsertDTO insertDTO)
        {
            var checkEntity = await _services.ApplicantService.GetApplicantByUserId(insertDTO.UserId);
            if (checkEntity != null) ModelState.AddModelError(nameof(insertDTO.UserId),
                $"The user with id {insertDTO.UserId} is already registerd");
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
                var result = await _services.ApplicantService.AddApplicant(insertDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }
        }

        [HttpPost("applicant")]
        public async Task<ActionResult<ApplicantViewExtendedDTO>> InsertCurrentApplicant(ApplicantInsertDTO insertDTO)
        {
            Claim? userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userClaim == null || !long.TryParse(userClaim.Value, out long userId)) return Unauthorized();

            insertDTO.UserId = userId;
            try
            {
                return await InsertApplicant(insertDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApplicantViewExtendedDTO>> InsertApplicant(ApplicantInsertDTO insertDTO)
        {
            if (insertDTO.UserId <= 0)
            {
                ModelState.AddModelError(nameof(insertDTO.UserId), "The userid is invalid");
            }
            try
            {
                return await Insert(insertDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }

        }

        [HttpPost("applicant/cv")]
        public async Task<ActionResult<ApplicantViewExtendedDTO>> SetApplicantCv(IFormFile file)
        {
            Claim? userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userClaim is null || !long.TryParse(userClaim.Value, out long userId)) return Unauthorized();
            try
            {
                string filename = FileProcessor.UploadPdf(file);
                var result = await _services.ApplicantService.UpdateApplicantCv(userId, filename);
                return Ok(result);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (FileUploadException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }
        }

        private async Task<ActionResult<ApplicantViewExtendedDTO>> Update(ApplicantUpdateDTO updateDTO)
        {
            if(!ModelState.IsValid)
            {
                foreach(var value in ModelState.Values)
                {
                    foreach(var error  in value.Errors)
                    {
                        _errors += error.ErrorMessage + " | ";
                    }
                }
                return BadRequest(_errors);
            }
            try
            {
                var result = await _services.ApplicantService.UpdateApplicant(updateDTO);
                return Ok(result);
            }
            catch(EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                return Problem(ex.Message) ;
            }
        }

        [HttpPut]
        public Task<ActionResult<ApplicantViewExtendedDTO>> UpdateInternal(ApplicantUpdateDTO updateDTO)
        {
            return Update(updateDTO);
        }

        [HttpPut("applicant")]
        public async Task<ActionResult<ApplicantViewExtendedDTO>> UpdateCurrentApplicant(ApplicantUpdateDTO updateDTO)
        {
            Claim? entityClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if(entityClaim == null || !long.TryParse(entityClaim.Value, out long applicantId) || !(applicantId == updateDTO.Id))
            {
                return Unauthorized();
            }
            return await  Update(updateDTO);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(long id)
        {
            try
            {
                var result = await _services.ApplicantService.DeleteApplicant(id);
                return Ok(result);
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

    }
}
