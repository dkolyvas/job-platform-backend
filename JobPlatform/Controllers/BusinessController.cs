using JobPlatform.Data;
using JobPlatform.DTO.Business;
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
    public class BusinessController : ControllerBase
    {
        private readonly IAppServices _services;
        private readonly ILogger<BusinessController> _logger;
        private string? _errors;

        public BusinessController(IAppServices services, ILogger<BusinessController> logger)
        {
            _services = services;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusinessViewDTO>>> GetAll([FromQuery] string? name, [FromQuery] string? email)
        {
            try
            {
                IEnumerable<BusinessViewDTO> result;
                if(name != null  || email != null)
                {
                    result = await _services.BusinessService.GetByNameOrEmail(name, email);
                }
                else
                {
                    result = await _services.BusinessService.GetAll();
                }
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BusinessViewDTO>> GetById(long id)
        {
            try
            {
                var result = await _services.BusinessService.GetById(id);
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

        [HttpGet("business")]
        public async Task<ActionResult<BusinessViewDTO>> GetUserBusiness()
        {
            Claim? claim = HttpContext.User.Claims.FirstOrDefault( c => c.Type == "EntityId");
            if(claim is null || ! long.TryParse(claim.Value, out long id))
            {

                return Unauthorized();
            }
            try
            {
                var result = await _services.BusinessService.GetById(id);
                return Ok(result);
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

        

        private async Task<ActionResult<BusinessViewDTO>> Insert(BusinessInsertDTO insertDTO)
        {
            var checkEntity = await _services.BusinessService.GetByUserId(insertDTO.UserId);
            if (checkEntity != null) ModelState.AddModelError(nameof(insertDTO.UserId), 
                $"The user with id {insertDTO.UserId} is already registered");
            if (!ModelState.IsValid)
            {
                foreach(var val in ModelState.Values)
                {
                    foreach(var error in val.Errors)
                    {
                        _errors += error.ErrorMessage + "| ";
                    }
                }
                return BadRequest(_errors);
            }
            try
            {
                var result = await _services.BusinessService.Insert(insertDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _errors = ex.Message;
                return Problem(_errors);
            }
        }

        [HttpPost("business")]
        public async Task<ActionResult<BusinessViewDTO>> InsertForClient(BusinessInsertDTO insertDTO)
        {
            Claim? userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if(userClaim == null || !long.TryParse(userClaim.Value, out long userId))
            {
                return Unauthorized();
            }
           
            insertDTO.UserId = userId;
            try
            {
                return await Insert(insertDTO);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }
            
        }

        [HttpPost]
        public async Task<ActionResult<BusinessViewDTO>> InsertForInternal(BusinessInsertDTO insertDTO)
        {
            if(insertDTO.UserId <= 0)
            {
                ModelState.AddModelError(nameof(insertDTO.UserId), "The submited userid is invalid");
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

        private async Task<ActionResult<BusinessViewDTO>> SetImage(IFormFile file, long userId)
        {
            
            try
            {
                string path = FileProcessor.UploadImage(file);
                BusinessUpdateImageDTO updateImageDTO = new()
                {
                    UserId = userId, ImageStr = path
                };
                var result = await _services.BusinessService.UpdateImage(updateImageDTO);
                return Ok(result);
            }
            catch(FileUploadException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
            catch(EntityNotFoundException ex)
            {
                
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }
        }

        [HttpPost("business/image")]
        public async Task<ActionResult<BusinessViewDTO>> SetImageForClient(IFormFile file)
        {
            Claim? userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if(userClaim is null || !long.TryParse(userClaim.Value, out long userId))
            {
                return Unauthorized();
            }
            return await SetImage(file, userId);
        }
        [HttpPost("image")]
        public async Task<ActionResult<BusinessViewDTO>> SetImageForInternal(IFormFile file, long userId)
        {
            return await SetImage(file, userId);
        }

        private async Task<ActionResult<BusinessViewDTO>> Update(BusinessUpdateDTO updateDTO)
        {
            if (!ModelState.IsValid)
            {
                foreach (var val in ModelState.Values)
                {
                    foreach (var error in val.Errors)
                    {
                        _errors += error.ErrorMessage + "| ";
                    }
                }
                return BadRequest(_errors);
            }
            try
            {
                var result = await _services.BusinessService.Update(updateDTO);
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

        [HttpPut]
        public async Task<ActionResult<BusinessViewDTO>> UpdateForInternal(BusinessUpdateDTO updateDTO)
        {
            return await Update(updateDTO);
        }

        [HttpPut("business")]
        public async Task<ActionResult<BusinessViewDTO>> UpdateForClient(BusinessUpdateDTO updateDTO)
        {
            Claim? entityClaim = HttpContext.User.Claims.FirstOrDefault( c => c.Type == "EntityId");
            if(entityClaim == null || !long.TryParse(entityClaim.Value, out long businessId) || updateDTO.Id != businessId)
            {
                return Unauthorized();
            }
            return await Update(updateDTO);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(long id)
        {
            try
            {
                bool result = await _services.BusinessService.Delete(id);
                return Ok(result);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(UnableToDeleteException ex)
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

    }
}
