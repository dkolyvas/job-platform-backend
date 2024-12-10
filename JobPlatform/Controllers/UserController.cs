using JobPlatform.Data;
using JobPlatform.DTO.User;
using JobPlatform.Exceptions;
using JobPlatform.Services;
using JobPlatform.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;

namespace JobPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAppServices _services;
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _config;
        private string errors = "";

        public UserController(IAppServices services, ILogger<UserController> logger, IConfiguration config)
        {
            _services = services;
            _logger = logger;
            _config = config;
        }

        private string CreateUserToken(UserViewDTO? user)
        {
            if (user is null) return "";

            string appSecurityKey = _config.GetValue<string>("AppSecurityKey")!;

            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSecurityKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claimsInfo = new List<Claim>();
            var tokenIssuer = "https://localhost:5001";

            claimsInfo.Add(new Claim(ClaimTypes.Name, user.Username!));

            claimsInfo.Add(new Claim(ClaimTypes.Role, user.Role!));



            claimsInfo.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()!));
            if (user.EntityId is not null)
            {
                claimsInfo.Add(new Claim("EntityId", user.EntityId.ToString()!));
            }




            var jwtSecurityToken = new JwtSecurityToken(tokenIssuer, null, claimsInfo, DateTime.UtcNow, DateTime.UtcNow.AddHours(3), signingCredentials);

            var userToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return userToken; //"Bearer " + userToken;

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDTO credentials)
        {
            if (!ModelState.IsValid)
            {
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
                UserViewDTO? user = await _services.UserService.Authenticate(credentials);
                string token = CreateUserToken(user);
                return Ok(new { token = token });
            }
            catch (AuthenticationErrorException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch(AccountBlockedException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserViewDTO>>> GetUsers([FromQuery] string? role)
        {
            IEnumerable<UserViewDTO> results;
            try
            {
                if (role == null)
                {
                    results = await _services.UserService.GetAll();
                }
                else
                {
                    results = await _services.UserService.GetAllByRole(role);
                }
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserViewDTO>> GetOne(long id)
        {
            try
            {
                var user = await _services.UserService.GetById(id);
                return Ok(user);
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

       
        [HttpGet("current")]
        public async Task<ActionResult<UserViewDTO>> GetCurrentUser()
        {
            Claim? authenticatedId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (authenticatedId == null) return NotFound();
            long id = int.Parse(authenticatedId.Value);
            try
            {
                var user = await _services.UserService.GetById(id);
                return Ok(user);
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

        private async Task<ActionResult<UserViewDTO>> Register(UserRegisterDTO registerDTO, bool isClient)
        {
            if(registerDTO.Password != registerDTO.ConfirmPassword)
            {
                ModelState.AddModelError(nameof(registerDTO.ConfirmPassword), "You must type twice the same password");
            }
            bool userExists = await _services.UserService.UserNameExists((registerDTO.Username is not null)?registerDTO.Username:"");
            if(userExists)
            {
                ModelState.AddModelError(nameof(registerDTO.Username), "The requested username already exists");
            }
            if(!ModelState.IsValid)
            {
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
                var newUser = await _services.UserService.Register(registerDTO);
                if (isClient)
                {
                    string token = CreateUserToken(newUser);
                    return Ok(new { token = token });
                }
                return CreatedAtAction("GetOne", new { id = newUser.Id }, newUser);

            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }
        }

        [HttpPost("admin/register")]
        public async Task<ActionResult<UserViewDTO>> RegisterInternalUser(UserRegisterDTO registerDTO)
        {
            registerDTO.RoleId = (int)Parameters.UserRoles.Internal;
            return await Register(registerDTO, false);
        }

        [HttpPost("business/register")]
        public async Task<ActionResult<UserViewDTO>> RegisterBusinessUser(UserRegisterDTO registerDTO)
        {
            registerDTO.RoleId = (int)Parameters.UserRoles.Business;
            return await Register(registerDTO, true);
        }

        [HttpPost("applicant/register")]
        public async Task<ActionResult<UserViewDTO>> RegisterApplicantUser(UserRegisterDTO registerDTO)
        {
            registerDTO.RoleId = (int)Parameters.UserRoles.Applicant;
            return await Register(registerDTO, true);
        }
        [HttpPut("email")]
        public async Task<ActionResult<UserViewDTO>> ChangeEmail(UserChangeEmailDTO dto)
        {
            Claim? authenticatedId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if(authenticatedId == null || int.Parse(authenticatedId.Value) != dto.Id)
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
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
                var user = await  _services.UserService.ChangeEmail(dto);
                return Ok(user);
            }
            catch(Exception ex)
            {
                if(ex is EntityNotFoundException || ex is AuthenticationErrorException)
                {
                    return Unauthorized(ex.Message);
                }
                else
                {
                    _logger.LogError(ex.Message, ex);
                    return Problem(ex.Message);
                }
            }


            
        }

        [HttpPut("password")]
        public async Task<ActionResult<UserViewDTO>> ChangePassword(UserChangePasswordDTO dto)
        {
            if(dto.NewPassword != dto.ConfirmPassword)
            {
                ModelState.AddModelError(nameof(dto.ConfirmPassword), "You must type twice the same passwords");
            }
            if(!ModelState.IsValid)
            {

                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors += error.ErrorMessage + " | ";
                    }
                }
                return BadRequest(errors);
            }
            Claim? authenticatedId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if(authenticatedId is null || int.Parse(authenticatedId.Value) != dto.Id)
            {
                return Unauthorized();
            }
            try
            {
                var user = await _services.UserService.ChangePassword(dto);
                return Ok(user);
            }
            catch (Exception ex)
            {
                if (ex is EntityNotFoundException || ex is AuthenticationErrorException)
                {
                    return Unauthorized(ex.Message);
                }
                else
                {
                    _logger.LogError(ex.Message, ex);
                    return Problem(ex.Message);
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(long id)
        {
            try
            {
                bool result = await _services.UserService.Delete(id);
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

        [HttpGet("reset")]
        public ActionResult<bool> Reset([FromQuery] string? username)
        {
            try
            {
                if (username == null) return BadRequest("no username specified");
                 _services.UserService.IssueRestorationCode(username);
                return Ok(true);
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

        [HttpPost("restore")]
        public async Task<ActionResult<UserViewDTO>> Restore(UserRestorationDTO restorationDTO)
        {
            if (!ModelState.IsValid)
            {
                foreach(var value in ModelState.Values)
                {
                    foreach(var error in value.Errors)
                    {
                        errors += $"{error.ErrorMessage} | ";
                    }
                }
                return BadRequest(errors);
            }
            try
            {
                var result = await _services.UserService.RestoreAccount(restorationDTO);
                return Ok(result);
            }
            catch(EntityNotFoundException ex)
            {
                return NotFound(ex.Message) ;

            }
            catch (AuthenticationErrorException)
            {
                return Unauthorized();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return BadRequest(ex.Message);
            }
        }


    }
}
