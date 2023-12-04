using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivesta.Model;

namespace Trivesta.Data.Interface
{
    public interface IRoom : IGeneric<Room>
    {
        Task<IEnumerable<Room>> NotDefaultRooms();
        Task<IEnumerable<Room>> DefaultRooms(string typeTag);
        Task<IEnumerable<Room>> NotDefaultRooms(string typeTag);
        Task<Room> GetByRoomCode(string code);
        Task<IEnumerable<Room>> GetByRoomsCode(string code);
        Task<Room> GetByRoomCodeTagOrID(string roomCodeTagOrID);
    }
}
