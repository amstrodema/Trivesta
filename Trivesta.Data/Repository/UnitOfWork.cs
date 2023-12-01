using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivesta.Data.Interface;

namespace Trivesta.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TrivestaContext _db;
        public UnitOfWork(TrivestaContext db, ISubscriber subscribers, IUser users, ICoinTransaction coinTransactions, IRoom rooms,
            IRoomType roomTypes, IRoomMember roomMembers, IMedia media, ILoginMonitor loginMonitors, INotification notifications)
        {
            _db = db;
            Subscribers = subscribers;
            Users = users;
            CoinTransactions = coinTransactions;
            Rooms = rooms;
            RoomTypes = roomTypes;
            RoomMembers = roomMembers;
            Media = media;
            LoginMonitors = loginMonitors;
            Notifications = notifications;
        }

        public ISubscriber Subscribers { get; }
        public IUser Users { get; }
        public ICoinTransaction CoinTransactions { get; }
        public IRoom Rooms { get; }
        public IRoomType RoomTypes { get; }
        public IRoomMember RoomMembers { get; }
        public IMedia Media { get; }
        public ILoginMonitor LoginMonitors { get; }
        public INotification Notifications { get; }

        public async Task<int> Commit() =>
          await _db.SaveChangesAsync();

        public void Rollback() => Dispose();

        public void Dispose() =>
            _db.DisposeAsync();
    }
}
