namespace ASGShared.Models
{
    public class DietaryRestrictions
    {
        public int Id { get; set; }
        public bool IsVegetarian { get; set; }
        public bool IsVegan { get; set; }
        public bool IsGlutenFree { get; set; }
        public bool IsPescatarian { get; set; }       
    }
}
