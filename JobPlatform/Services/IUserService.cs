using JobPlatform.DTO.User;

namespace JobPlatform.Services
{
    public interface IUserService
    {
        public Task<IEnumerable<UserViewDTO>> GetAll();
        public Task<IEnumerable<UserViewDTO>> GetAllByRole(string role);
        public Task<UserViewDTO> GetById(long id);
        public Task<UserViewDTO> GetByUsername(string username);
        public Task<bool> UserNameExists(string username);

        public Task<UserViewDTO?> Authenticate(UserLoginDTO credentials);
        public Task<UserViewDTO> Register(UserRegisterDTO registerDTO);
        public Task<UserViewDTO> ChangePassword(UserChangePasswordDTO changePasswordDTO);
        public Task<UserViewDTO> ChangeEmail(UserChangeEmailDTO changeEmailDTO);
        public void IssueRestorationCode(string username);
        public Task<UserViewDTO?> RestoreAccount(UserRestorationDTO restorationDTO);
        public Task<bool> Delete(long id);
    }
}
