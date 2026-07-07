namespace ExamTwo.Models
{
    public class CoinInventory
    {
        private readonly Dictionary<int, int> _coins;

        public CoinInventory(Dictionary<int, int> coins)
        {
            _coins = coins;
        }

        public IReadOnlyDictionary<int, int> Coins => _coins;

        public bool MakeChange(int amount)
        {
            var remaining = WithdrawCoins(amount, out var usedCoins);

            if (remaining == 0)
            {
                PrintSuccess(amount, usedCoins);
            }
            else
            {
                RollBack(usedCoins);
                Console.WriteLine("Falló al realizar la compra");
            }

            return remaining == 0;
        }

        private int WithdrawCoins(int amount, out Dictionary<int, int> usedCoins)
        {
            var remaining = amount;
            usedCoins = new Dictionary<int, int>(); // Track the coins used for change

            foreach (var coin in _coins.Keys.OrderByDescending(c => c)) // Iterate through coins in descending order (500, 100, 50, 25)
            {
                while (remaining >= coin && _coins[coin] > 0)
                {
                    remaining -= coin; // Deduct the coin value from the remaining amount
                    _coins[coin]--; // Deduct the coin from the inventory

                    if (!usedCoins.ContainsKey(coin)) // Initialize the count for this coin if not already present
                        usedCoins[coin] = 0; // Increment the count of this coin used for change
                    usedCoins[coin]++; // Increment the count of this coin used for change
                }
            }

            return remaining;
        }

        private void PrintSuccess(int amount, Dictionary<int, int> usedCoins)
        {
            Console.WriteLine("Su vuelto es de: " + amount + " colones. Desglose:");
            foreach (var kvp in usedCoins)
            {
                Console.WriteLine($"{kvp.Value} monedas de {kvp.Key} colones");
            }
        }

        private void RollBack(Dictionary<int, int> usedCoins)
        {
            foreach (var kvp in usedCoins) // Roll back the coins used, since the change could not be completed
                _coins[kvp.Key] += kvp.Value;
        }
    }
}
