using JobPlatform.DTO.Subscription;
using JobPlatform.Exceptions;
using JobPlatform.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly IAppServices _services;
        private readonly ILogger<SubscriptionController> _logger;
        private string? _errors;

        public SubscriptionController(IAppServices services, ILogger<SubscriptionController> logger)
        {
            _services = services;
            _logger = logger;
        }

        [HttpGet("business/{id}")]
        public async Task<ActionResult<SubscriptionViewDTO>> GetBusinessSubscription(long id)
        {
            Claim? entityClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if(entityClaim is null || !long.TryParse(entityClaim.Value, out long businesesId))
            {
                return Unauthorized();
            }
            try
            {
                var result = await _services.SubscriptionService.GetSubscription(id);
                if (result.BusinessId != businesesId) return Unauthorized();
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

        [HttpGet("{id}")]
        public async Task<ActionResult<SubscriptionViewDTO>> GetSubscription(long id)
        {
            try
            {
                var result = await _services.SubscriptionService.GetSubscription(id);
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

        private async Task<ActionResult<IEnumerable<SubscriptionViewDTO>>> GetSubscriptions(long businessId)
        {
            try
            {
                var result = await _services.SubscriptionService.GetBusinessSubscriptions(businessId);
                return Ok(result);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message, e);
                return Problem(e.Message);
            }
        }

        [HttpGet("business")]
        public async Task<ActionResult<IEnumerable<SubscriptionViewDTO>>> GetSubscriptionsBusiness()
        {
            Claim? entityClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if (entityClaim is null || !long.TryParse(entityClaim.Value, out long businesesId))
            {
                return Unauthorized();
            }
            return await GetSubscriptions(businesesId);

        }

        [HttpGet("bybusiness/{businessId}")]
        public async Task<ActionResult<IEnumerable<SubscriptionViewDTO>>> GetSubscriptionsInternal(long businessId)
        {
            return await GetSubscriptions(businessId);
        }


        private async Task<ActionResult<SubscriptionViewDTO>> AddOrRenew(SubscriptionInsertDTO dto)
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
                var result = await _services.SubscriptionService.AddOrRenewSubscription(dto);
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }
        }

        [HttpPost("business")]
        public async Task<ActionResult<SubscriptionViewDTO>> AddOrRenewForBusiness(SubscriptionInsertDTO dto)
        {
            Claim? entityClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if (entityClaim is null || !long.TryParse(entityClaim.Value, out long businesesId))
            {
                return Unauthorized();
            }
            dto.BusinessId = businesesId;
            return await AddOrRenew(dto);

        }

        [HttpPost]
        public async Task<ActionResult<SubscriptionViewDTO>> AddOrRenewInternal(SubscriptionInsertDTO dto)
        {
            return await AddOrRenewInternal(dto);
        }

        [HttpPut]
        public async Task<ActionResult<SubscriptionViewDTO>> Update(SubscriptionUpdateDTO dto)
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
                var result = await _services.SubscriptionService.Update(dto);
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
        [HttpPut("cancel/{id}")]
        public async Task<ActionResult<SubscriptionViewDTO>> CancelInternal(long id)
        {
            try
            {
                var result = await _services.SubscriptionService.CancelSubscription(id);
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
        [HttpPut("business/cancel/{id}")]
        public async Task<ActionResult<SubscriptionViewDTO>> CancelBusiness(long id)
        {
            Claim? entityClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if (entityClaim is null || !long.TryParse(entityClaim.Value, out long businesesId))
            {
                return Unauthorized();
            }
            try
            {
                var result = await _services.SubscriptionService.CancelSubscription(id, businesesId);
                return Ok(result);
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
                _logger.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(long id)
        {
            try
            {
                bool result = await _services.SubscriptionService.DeleteSubscription(id);
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
