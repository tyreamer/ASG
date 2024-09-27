namespace ASGShared.Models
{
    public class Budget
    {
        public int Amount { get; set; } = 0;
        public string Currency { get; set; } = "USD"; // Default to USD

        public Budget() { }

        public Budget(int amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }
    }
}
