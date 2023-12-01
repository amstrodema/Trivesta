namespace Trivesta.Model
{
    public class Subscriber
    {
        public Guid ID { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
    }
}