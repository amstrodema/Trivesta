namespace Trivesta.Model
{
    public class CoinTransaction
    {
        public Guid ID { get; set; }
        public Guid UserID { get; set; }
        public bool IsCredit { get; set; }
        public string TrxID { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}