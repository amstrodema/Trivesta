namespace Trivesta.Model
{
    public class Media
    {
        public Guid ID { get; set; }
        public Guid UserID { get; set; }
        public Guid ItemID { get; set; }
        public string Item { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}