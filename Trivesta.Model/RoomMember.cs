namespace Trivesta.Model
{
    public class RoomMember
    {
        public Guid ID { get; set; }
        public Guid RoomID { get; set; }
        public Guid UserID { get; set; }
        public int Duration { get; set; } //in hours
    }
}