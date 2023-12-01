namespace Trivesta.Model
{
    public class Room
    {
        public Guid ID { get; set; }
        public Guid RoomTypeID { get; set; }
        public string RoomTypeTag { get; set; }
        public string RoomType { get; set; }
        public string Tag { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RentType { get; set; } = string.Empty;
        public decimal RoomCost { get; set; }
        public string RentFreq { get; set; } = string.Empty;
        public decimal RentAmt { get; set; }
        public string RelationshipLimit { get; set; } = string.Empty;
        public string AgeLimit { get; set; } = string.Empty;
        public string ReligiousLimit { get; set; } = string.Empty;
        public string GenderLimit { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string IsDefault { get; set; } = string.Empty;
        public bool IsFree { get; set; }
        public bool IsBarred { get; set; }

        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
    }
}