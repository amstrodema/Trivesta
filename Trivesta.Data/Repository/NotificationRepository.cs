using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivesta.Data.Interface;
using Trivesta.Model;

namespace Trivesta.Data.Repository
{
    public class NotificationRepository : GenericRepository<Notification>, INotification
    {
        public NotificationRepository(TrivestaContext db) : base(db) { }
        public async Task<int> CheckForNotification(Guid userID)
        {
            return (await GetBy(n => n.ReceiverID == userID && !n.IsRead && n.IsActive)).Count();
        }
        public async Task<IEnumerable<Notification>> GetByUserID(Guid userID)
        {
            return await GetBy(n => n.ReceiverID == userID && n.IsActive);
        }
    }
}
