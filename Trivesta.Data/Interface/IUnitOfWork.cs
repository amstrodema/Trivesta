using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivesta.Data.Interface
{
    public interface IUnitOfWork
    {
        public ISubscriber Subscribers { get; }
        public IUser Users { get; }
        public ICoinTransaction CoinTransactions { get; }
        public IRoom Rooms { get; }
        public IRoomType RoomTypes { get; }
        public IRoomMember RoomMembers { get; }
        public IMedia Media { get; }
        public ILoginMonitor LoginMonitors { get; }
        public INotification Notifications { get; }
        Task<int> Commit();
        void Rollback();
    }
}
