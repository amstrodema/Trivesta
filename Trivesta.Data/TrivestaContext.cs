using Microsoft.EntityFrameworkCore;
using Trivesta.Model;

namespace Trivesta.Data
{
    public class TrivestaContext : DbContext
    {
        public TrivestaContext(DbContextOptions<TrivestaContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoomType>().Property(o => o.Cost).HasPrecision(14, 2);
            modelBuilder.Entity<Room>().Property(o => o.RoomCost).HasPrecision(14, 2);
            modelBuilder.Entity<Room>().Property(o => o.RentAmt).HasPrecision(14, 2);
            modelBuilder.Entity<CoinTransaction>().Property(o => o.Amount).HasPrecision(14, 2);
        }
        public virtual DbSet<CoinTransaction> CoinTransactions { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<RoomType> RoomTypes { get; set; }
        public virtual DbSet<RoomMember> RoomMembers { get; set; }
        public virtual DbSet<Media> Medias { get; set; }
        public virtual DbSet<Subscriber> Subscribers { get; set; }
        public virtual DbSet<LoginMonitor> LoginMonitors { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
    }
   
    }