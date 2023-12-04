using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivesta.Model;

namespace Trivesta.Data.Interface
{
    public interface IUser : IGeneric<User>
    {
        Task<User> GetUserByUserNameOrEmail(string usernameOrEmail);
        Task<User> GetActiveUserByUserName(string username);
        Task<User> GetUserByUserNameOrEmail(string username, string email, string tel);
        Task<User> GetActiveUserByUserID(Guid userID);
        Task<IEnumerable<User>> GetReferrals(string username);
        Task<User> GetUserByUserNameOrEmail(string username, string email);
    }
}
