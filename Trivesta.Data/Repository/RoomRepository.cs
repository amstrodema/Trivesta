using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivesta.Data.Interface;
using Trivesta.Model;

namespace Trivesta.Data.Repository
{
    public class RoomRepository : GenericRepository<Room>, IRoom
    {
        public RoomRepository(TrivestaContext db) : base(db) { }
    }
}
