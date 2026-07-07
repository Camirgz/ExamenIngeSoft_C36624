using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExamTwo.Controllers
{
    public class CoffeeMachineController : Controller
    {

        private readonly Database _db;

        public CoffeeMachineController(Database db)
        {
            _db = db;
        }

        [HttpGet("getCoffees")]
        public ActionResult<Dictionary<string, int>> GetCoffeePrices()
        {
            return Ok(_db.coffeeStock);
        }

        [HttpGet("getCoffeePricesInCents")]
        public ActionResult<Dictionary<string, int>> GetCoffeePricesInCents()
        {
            return Ok(_db.coffeePrice);
        }

        [HttpGet("getQuantity")]
        public ActionResult<Dictionary<string, int>> GetQuantity()
        {
            return Ok(_db.coinInventory);
        }

        [HttpPost("buyCoffee")]
        public ActionResult<string> BuyCoffee([FromBody] OrderRequest request)
        {
            if (request.Order == null || request.Order.Count == 0)
                return BadRequest("Orden vacía.");

            if (request.Payment.TotalAmount <= 0)
                return BadRequest("Dinero insuficiente ");

            try
            {
                var totalCost = request.Order.Sum(o => _db.coffeePrice.First(c => c.Key == o.Key).Value * o.Value);

                if (request.Payment.TotalAmount < totalCost)
                { 
                    return BadRequest("Dinero insuficiente ");
                }


                foreach (var coffee in request.Order)
                {
                    var selected = _db.coffeeStock.First(c => c.Key == coffee.Key).Key;
                    if (coffee.Value > _db.coffeeStock[selected])
                    {
                        return $"No hay suficientes {selected} en la máquina.";
                    }
                    _db.coffeeStock[selected] -= coffee.Value;
                }

                var change = request.Payment.TotalAmount - totalCost;
                String result = $"Su vuelto es de: {change} colones. Desglose:";

                foreach (var coin in _db.coinInventory.Keys.OrderByDescending(c => c))
                {
                    var count = Math.Min(change / coin, _db.coinInventory[coin]);
                    if (count > 0)
                    {
                        result +=  $" {count} moneda de {coin},  ";              
                        change -= coin * count;
                    }
                }


                if (change > 0)
                {
                    return StatusCode(500, "No hay suficiente cambio en la máquina.");
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class OrderRequest
    {
        public Dictionary<string, int> Order { get; set; }
        public Payment Payment { get; set; }
    }

    public class Payment
    {
        public int TotalAmount { get; set; }
        public List<int> Coins { get; set; }
        public List<int> Bills { get; set; }
    }
}
