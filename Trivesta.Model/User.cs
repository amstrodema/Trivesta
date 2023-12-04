namespace Trivesta.Model
{
    public class User
    {
        public Guid ID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Tel { get; set; } = string.Empty;
        public string ReferredBy { get; set; } = string.Empty;
        public string EmailVerCode { get; set; } = string.Empty;
        public string Religion { get; set; } = string.Empty;
        public Guid AppCode { get; set; }
        public decimal CoinBonus { get; set; }
        public DateTime DOB { get; set; } 
        public bool IsMarried { get; set; }
        public bool IsMale { get; set; }
        public bool IsBarred { get; set; }
        public bool IsActive { get; set; }
        public bool IsDev { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBanned { get; set; }
        public bool IsMailVerified { get; set; }
        public string Password { get; set; } = string.Empty;
        public string PasswordVer { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
    }
}