namespace ASGShared.Models
{
    public class Budget
    {
        public decimal Amount { get; set; } = 0.0m;
        public string Currency { get; set; } = "USD"; // Default to USD

        public Budget() { }

        public Budget(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }
    }
}
