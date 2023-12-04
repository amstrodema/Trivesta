using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivesta.Data.Interface;
using Trivesta.Model;

namespace Trivesta.Data.Repository
{
    public class UserRepository : GenericRepository<User>, IUser
    {
        public UserRepository(TrivestaContext db) : base(db) { }
        public async Task<User> GetActiveUserByUserName(string username)
        {
            return await GetOneBy(u => u.Username == username && u.IsActive);
        }
        public async Task<User> GetActiveUserByUserID(Guid userID)
        {
            return await GetOneBy(u => u.ID == userID && u.IsActive);
        }
        public async Task<User> GetUserByUserNameOrEmail(string usernameOrEmail)
        {
            return await GetOneBy(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);
        }
        public async Task<User> GetUserByUserNameOrEmail(string username, string email)
        {
            return await GetOneBy(u => u.Username == username || u.Email == email);
        }
        public async Task<User> GetUserByUserNameOrEmail(string username, string email, string tel)
        {
            return await GetOneBy(u => u.Username == username || u.Email == username || u.Tel == tel);
        }
        public async Task<IEnumerable<User>> GetReferrals(string username)
        {
            return await GetBy(u => u.ReferredBy == username);
        }
    }
}
