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
        public async Task<IEnumerable<Room>> NotDefaultRooms()
        {
            return await GetBy(p => !p.IsDefault);
        }
        public async Task<IEnumerable<Room>> NotDefaultRooms(string typeTag)
        {
            return await GetBy(p => !p.IsDefault && p.RoomTypeTag == typeTag);
        }
        public async Task<IEnumerable<Room>> DefaultRooms(string typeTag)
        {
            return await GetBy(p => p.IsDefault && p.RoomTypeTag == typeTag);
        }
        public async Task<IEnumerable<Room>> GetByRoomsCode(string code)
        {
            return await GetBy(p =>  p.Code == code);
        }
        public async Task<Room> GetByRoomCode(string code)
        {
            return await GetOneBy(p =>  p.Code == code);
        }
        public async Task<Room> GetByRoomCodeTagOrID(string roomCodeTagOrID)
        {
            try
            {
                return await GetOneBy(p => p.ID == Guid.Parse(roomCodeTagOrID));
            }
            catch (Exception)
            {
                return await GetOneBy(p => p.Code == roomCodeTagOrID || p.Tag == roomCodeTagOrID || p.Code == roomCodeTagOrID);
            }            
        }
    }
}
