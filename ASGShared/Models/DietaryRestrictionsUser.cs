namespace ASGShared.Models
{
    public class DietaryRestrictionsUser
    {
        public string UserId { get; set; }
        public User User { get; set; }

        public int DietaryRestrictionsId { get; set; }
        public DietaryRestrictions DietaryRestrictions { get; set; }
    }
}