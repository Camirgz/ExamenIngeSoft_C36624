using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ExamTwo.Services;

namespace ExamTwo.Controllers
{
    public class CoffeeMachineController : Controller
    {

        private readonly Database _db;
        private readonly CoffeeMachineService _service;

        public CoffeeMachineController(Database db, CoffeeMachineService service)
        {
            _db = db;
            _service = service;
        }

        [HttpPost("calculateTotal")]
        public ActionResult<int> CalculateTotal([FromBody] Dictionary<string, int> order)
        {
            try
            {
                return Ok(_service.CalculateTotalCost(order));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("insertMoney")]
        public ActionResult<int> InsertMoney([FromBody] Payment payment)
        {
            try
            {
                return Ok(_service.InsertMoney(payment.Coins, payment.Bills));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
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
            try
            {
                var outcome = _service.BuyCoffee(request);

                if (outcome.OutOfService)
                    return StatusCode(500, outcome.Message);

                if (!outcome.Success)
                    return BadRequest(outcome.Message);

                return Ok(outcome.Message);
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
