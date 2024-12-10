using AutoMapper;
using JobPlatform.DTO.User;
using JobPlatform.Repositories;
using JobPlatform.Util;
using JobPlatform.Exceptions;

using JobPlatform.Data;

namespace JobPlatform.Services
{

    public class UserService : IUserService
    {
        private readonly IUnitOfWork _repositories;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(IUnitOfWork repositories, IMapper mapper, IConfiguration configuration)
        {
            _repositories = repositories;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<UserViewDTO?> Authenticate(UserLoginDTO credentials)
        {
            UserViewDTO result = new();
            long? entityId = null;
            if (credentials.UserName == "admin" && credentials.Password == _configuration.GetValue<string>("AdminPassword"))
            {
                result = new()
                {
                    Username = "admin",
                    Role = "admin",
                    Id = -1
                };
                return result;
            }
            var user = await _repositories.UserRepository.FindUserByUsername(credentials.UserName!);
            if (user == null)
            {

                throw new AuthenticationErrorException("username");
            }
            if(user.UnauthorizedCount >= 5)
            {
                throw new AccountBlockedException();
            }
            else if (!Encryption.VerifyPassword(credentials.Password!, user.Password!))
            {
                _repositories.UserRepository.RegisterFailedLogin(user);
                throw new AuthenticationErrorException("password");
            }
            else
            {
                if (user.Role == "business")
                {
                    var business = user.Business;
                    if (business != null) entityId = business.Id;
                }
                if (user.Role == "applicant")
                {
                    var applicant = user.Applicant;
                    if (applicant != null) entityId = applicant.Id;
                }
                result = _mapper.Map<UserViewDTO>(user);
                result.EntityId = entityId;
            }
                return result;

            }

        public async Task<UserViewDTO> ChangeEmail(UserChangeEmailDTO changeEmailDTO)
        {
            var user = await _repositories.UserRepository.FindById(changeEmailDTO.Id);
            if (user is null) throw new EntityNotFoundException("user");
            if (!Encryption.VerifyPassword(changeEmailDTO.Password!, user.Password!)) throw new AuthenticationErrorException("password");
            user = _repositories.UserRepository.ChangeUserEmail(user, changeEmailDTO.Email!);
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<UserViewDTO>(user);
        }

        public async Task<UserViewDTO> ChangePassword(UserChangePasswordDTO changePasswordDTO)
        {
            var user = await _repositories.UserRepository.FindById(changePasswordDTO.Id);
            if (user is null) throw new EntityNotFoundException("user");
            if (!Encryption.VerifyPassword(changePasswordDTO.Password!, user.Password!)) throw new AuthenticationErrorException("password");
            user = _repositories.UserRepository.ChangeUserPassword(user,Encryption.EncryptPassword( changePasswordDTO.NewPassword!));
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<UserViewDTO>(user);
        }

        public async Task<bool> Delete(long id)
        {
            bool result = await _repositories.UserRepository.Delete(id);
            if (!result) throw new EntityNotFoundException("user");
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return result;
        }

        public async Task<IEnumerable<UserViewDTO>> GetAll()
        {
            var data = await _repositories.UserRepository.FindAll();
            return _mapper.Map<IEnumerable<UserViewDTO>>(data);
        }

        public async Task<IEnumerable<UserViewDTO>> GetAllByRole(string role)
        {
            var data = await _repositories.UserRepository.FindUsersByRole(role);
            return _mapper.Map<IEnumerable<UserViewDTO>>(data);
        }

        public async Task<UserViewDTO> GetById(long id)
        {
            var data = await _repositories.UserRepository.FindById(id);
            if (data is null) throw new EntityNotFoundException("user");
            return _mapper.Map<UserViewDTO>(data);
        }

        public async Task<UserViewDTO> GetByUsername(string username)
        {
            var data = await _repositories.UserRepository.FindUserByUsername(username);
            if (data is null) throw new EntityNotFoundException("user");
            return _mapper.Map<UserViewDTO>(data);
        }

        public async void IssueRestorationCode(string username)
        {
            var user = await _repositories.UserRepository.IssueRestorationCode(username);
            if(user is null) throw new EntityNotFoundException("user");
        }

        public async Task<UserViewDTO> Register(UserRegisterDTO registerDTO)
        {
            string role = "";
            switch (registerDTO.RoleId)
            {
                case (int)Parameters.UserRoles.Internal:
                    role = "internal";
                    break;
                case (int)Parameters.UserRoles.Business:
                    role = "business";
                    break;
                default:
                    role = "applicant";
                    break;
            }
            User user = new()
            {
                Username = registerDTO.Username,
                Password = Encryption.EncryptPassword(registerDTO.Password!),
                Email = registerDTO.Email,
                Role = role,

            };
            user = await _repositories.UserRepository.AddOne(user);
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<UserViewDTO>(user);
        }

        public async Task<UserViewDTO?> RestoreAccount(UserRestorationDTO restorationDTO)
        {
            User? user = await _repositories.UserRepository.FindUserByUsername(restorationDTO.UserName);
            if (user == null) throw new EntityNotFoundException("user");
            if (user.RestoreCode != restorationDTO.RestorationCode) throw new AuthenticationErrorException("restoration code");
            var result =  _repositories.UserRepository.ChangeUserPassword(user, Encryption.EncryptPassword(restorationDTO.NewPassword!));
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<UserViewDTO?>(user);
        }

        public async Task<bool> UserNameExists(string username)
        {
            var data = await _repositories.UserRepository.FindUserByUsername(username);
            return data is not  null;
        }
    }
}
