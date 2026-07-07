using ExamTwo.Models;

namespace ExamTwo.Controllers
{
    public class Database
    {
        public List<CoffeeType> Coffees { get; } = new List<CoffeeType>
        {
            new CoffeeType("Americano", 950, 10),
            new CoffeeType("Cappuccino", 1200, 8),
            new CoffeeType("Lates", 1350, 10),
            new CoffeeType("Mocaccino", 1500, 15)
        };

        public CoinInventory ChangeCoins { get; } = new CoinInventory(new Dictionary<int, int>
        {
            { 500, 20 },
            { 100, 30 },
            { 50, 50 },
            { 25, 25 }
        });

        public Dictionary<string, int> coffeeStock => Coffees.ToDictionary(c => c.Name, c => c.Stock);
        public Dictionary<string, int> coffeePrice => Coffees.ToDictionary(c => c.Name, c => c.Price);
        public Dictionary<int, int> coinInventory => ChangeCoins.Coins.ToDictionary(kv => kv.Key, kv => kv.Value);
    }
}
