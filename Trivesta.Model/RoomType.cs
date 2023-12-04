namespace Trivesta.Model
{
    public class RoomType
    {
        public Guid ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public int DecayGraceInMinutes  { get; set; }
        public decimal Cost  { get; set; }

        public bool IsApprovalRequired { get; set; }
        public bool IsMustLogIn { get; set; }
        public bool IsMustBeUser { get; set; }
        public bool IsMustCharge { get; set; }
        public bool IsActive { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}