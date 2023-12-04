namespace Trivesta.Model
{
    public class LoginMonitor
    {
        public Guid ID { get; set; }
        public Guid UserID { get; set; }
        public DateTime TimeLogged { get; set; }
        public Guid ClientCode { get; set; }
        public string Guest { get; set; } = string.Empty;
        public decimal GuestCoin { get; set; }

        public bool IsApproved { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
    }
}