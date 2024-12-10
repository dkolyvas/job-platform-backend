using JobPlatform.Data;
using JobPlatform.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace JobPlatform.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(JobplatformContext db) : base(db)
        {
            
        }

        public async Task<User?> FindUserByUsername(string username)
        {
            var user = await _db.Users.Where(u => u.Username == username).Include(u => u.Business).Include(u => u.Applicant).FirstOrDefaultAsync();
            return user;
        }

        public async Task<IEnumerable<User>> FindUsersByRole(string role)
        {
            return await _db.Users.Where(u => u.Role == role).ToListAsync();
        }

        public User ChangeUserPassword(User user, string newPassword)
        {
            user.Password = newPassword;
            user.UnauthorizedCount = 0;
            _table.Entry(user).State = EntityState.Modified;
            return user;
        }

        public User ChangeUserEmail(User user, string newEmail)
        {
            user.Email = newEmail;
            _table.Entry(user).State = EntityState.Modified;
            return user;
        }


        public async void RegisterFailedLogin(User user)
        {
            if (user.UnauthorizedCount is null) user.UnauthorizedCount = 1;
            else user.UnauthorizedCount++;
            _table.Entry(user).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task<User?> IssueRestorationCode(string username)
        {
            var user = await _db.Users.Where(u => u.Username == username).FirstOrDefaultAsync();
            
            if (user is not null)
            {
                string str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890*/+-!@#$%^&*";
                Random rnd = new();
                string restorationCode = "";
                for (int i = 0; i < 16; i++)
                {
                    int index = rnd.Next(str.Length);
                    restorationCode += str[index];
                }
                user.RestoreCode = restorationCode;
                _table.Entry(user).State = EntityState.Modified;
                
            }
            return user;
        }

      


        public override async Task<bool> Delete(long id)
        {
            var user = await _db.Users.FindAsync(id);
            if(user == null) return false;
            if (user.Business != null || user.Applicant != null) throw new UnableToDeleteException();
            _db.Users.Remove(user);
            return true;
        }
    }
}
