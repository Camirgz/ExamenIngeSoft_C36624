using ExamTwo.Controllers;

namespace ExamTwo.Services
{
    public class CoffeeMachineService
    {
        private static readonly HashSet<int> ValidDenominations = new HashSet<int> { 500, 100, 50, 25, 1000 }; // Bills and coins 

        private readonly Database _db;

        public CoffeeMachineService(Database db)
        {
            _db = db;
        }

        public int CalculateTotalCost(Dictionary<string, int> order)
        {
            if (order == null || order.Count == 0)
                throw new ArgumentException("Orden vacía.");

            // Calculate the total cost of the order by summing the price of each coffee type multiplied by its quantity
            return order.Sum(o => _db.coffeePrice.First(c => c.Key == o.Key).Value * o.Value);
        }

        public int InsertMoney(List<int> coins, List<int> bills)
        {
            var inserted = (coins ?? new List<int>()).Concat(bills ?? new List<int>()).ToList();

            var invalid = inserted.FirstOrDefault(d => !ValidDenominations.Contains(d));
            if (invalid != 0)
                throw new ArgumentException($"Denominación no aceptada: {invalid}");

            return inserted.Sum();
        }

        public PurchaseOutcome BuyCoffee(OrderRequest request)
        {
            if (request.Order == null || request.Order.Count == 0)
                return new PurchaseOutcome(false, false, "Orden vacía.");

            var totalCost = CalculateTotalCost(request.Order);

            if (request.Payment.TotalAmount < totalCost)
                return new PurchaseOutcome(false, false, "Dinero insuficiente ");

            foreach (var coffee in request.Order)
            {
                var selected = _db.coffeeStock.First(c => c.Key == coffee.Key).Key;
                if (coffee.Value > _db.coffeeStock[selected])
                    return new PurchaseOutcome(false, false, $"No hay suficientes {selected} en la máquina.");
            }

            var change = request.Payment.TotalAmount - totalCost;
            var before = _db.coinInventory;

            if (!_db.ChangeCoins.MakeChange(change))
                return new PurchaseOutcome(false, true, "No hay suficiente cambio en la máquina.");

            foreach (var coffee in request.Order)
                _db.Coffees.First(c => c.Name == coffee.Key).Deduct(coffee.Value);

            var after = _db.coinInventory;
            var desglose = string.Join(" ", before.OrderByDescending(kv => kv.Key)
                .Where(kv => kv.Value - after[kv.Key] > 0)
                .Select(kv => $"{kv.Value - after[kv.Key]} moneda de {kv.Key}"));

            return new PurchaseOutcome(true, false, $"Su vuelto es de: {change} colones. Desglose: {desglose}");
        }
    }

    public class PurchaseOutcome
    {
        public bool Success { get; }
        public bool OutOfService { get; }
        public string Message { get; }

        public PurchaseOutcome(bool success, bool outOfService, string message)
        {
            Success = success;
            OutOfService = outOfService;
            Message = message;
        }
    }
}
