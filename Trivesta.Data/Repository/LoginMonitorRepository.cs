using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivesta.Data.Interface;
using Trivesta.Model;

namespace Trivesta.Data.Repository
{
    public class LoginMonitorRepository : GenericRepository<LoginMonitor>, ILoginMonitor
    {
        public LoginMonitorRepository(TrivestaContext db) : base(db) { }
        public async Task<LoginMonitor> GetMonitorByUserID(Guid userID, Guid appID)
        {
            return await GetOneBy(u => u.UserID == userID && u.ClientCode == appID);
        }
        public async Task<LoginMonitor> GetMonitorByUserIDOrGuest(Guid userID, Guid appID, string username)
        {
            return await GetOneBy(u => (u.UserID == userID || u.Guest == username) && u.ClientCode == appID);
        }
        public async Task<LoginMonitor> GetMonitorByUserIDOnly(Guid userID)
        {
            return await GetOneBy(u => u.UserID == userID);
        }
        public async Task<LoginMonitor> GetMonitorByGuest(string guest)
        {
            return await GetOneBy(u => u.Guest == guest);
        }
    }
}
