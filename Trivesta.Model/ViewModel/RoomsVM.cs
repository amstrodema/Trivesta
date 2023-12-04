using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivesta.Model.ViewModel
{
    public class RoomsVM
    {
        public IEnumerable<Room> Rooms { get; set; }
        public IEnumerable<Room> DefaultRooms { get; set; }
        public string t { get; set; }
        public decimal Cost { get; set; } = 100;
        public Room Room { get; set; } 
        public RoomType RoomType { get; set; }
        public User ? LoggedInUser { get; set; }
        public User ? Creator { get; set; }
    }
}
