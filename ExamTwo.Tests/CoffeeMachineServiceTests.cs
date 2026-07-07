using ExamTwo.Controllers;
using ExamTwo.Services;

namespace ExamTwo.Tests;

public class CoffeeMachineServiceTests
{
    private static CoffeeMachineService CreateService() => new CoffeeMachineService(new Database());

    [Fact]
    public void CalculateTotalCost()
    {
        var service = CreateService();

        var total = service.CalculateTotalCost(new Dictionary<string, int> { { "Americano", 2 }, { "Lates", 1 } });

        Assert.Equal(950 * 2 + 1350, total);
    }

    [Fact]
    public void CalculateTotalCost_EmptyOrder()
    {
        var service = CreateService();

        Assert.Throws<ArgumentException>(() => service.CalculateTotalCost(new Dictionary<string, int>()));
    }

    [Fact]
    public void InsertMoney_ValidDenominations()
    {
        var service = CreateService();

        var total = service.InsertMoney(new List<int> { 500, 100 }, new List<int> { 1000 });

        Assert.Equal(1600, total);
    }

    [Fact]
    public void InsertMoney_InvalidDenomination()
    {
        var service = CreateService();

        Assert.Throws<ArgumentException>(() => service.InsertMoney(new List<int> { 10 }, new List<int>()));
    }

    [Fact]
    public void BuyCoffee_QuantityAboveStock_ReturnsFailure()
    {
        var service = CreateService();
        var request = new OrderRequest
        {
            Order = new Dictionary<string, int> { { "Cappuccino", 100 } },
            Payment = new Payment { TotalAmount = 1_000_000 }
        };

        var outcome = service.BuyCoffee(request);

        Assert.False(outcome.Success);
        Assert.False(outcome.OutOfService);
    }

    [Fact]
    public void BuyCoffee_SuccessfulPurchase_DeductsStock()
    {
        var db = new Database();
        var service = new CoffeeMachineService(db);
        var request = new OrderRequest
        {
            Order = new Dictionary<string, int> { { "Americano", 1 } },
            Payment = new Payment { TotalAmount = 1000 }
        };

        var outcome = service.BuyCoffee(request);

        Assert.True(outcome.Success);
        Assert.Equal(9, db.Coffees.First(c => c.Name == "Americano").Stock);
    }

    [Fact]
    public void BuyCoffee_NoStockLeft()
    {
        var db = new Database();
        var service = new CoffeeMachineService(db);
        var stock = db.Coffees.First(c => c.Name == "Cappuccino").Stock;

        var soldOut = service.BuyCoffee(new OrderRequest
        {
            Order = new Dictionary<string, int> { { "Cappuccino", stock } },
            Payment = new Payment { TotalAmount = stock * 1200 }
        });
        Assert.True(soldOut.Success);
        Assert.Equal(0, db.Coffees.First(c => c.Name == "Cappuccino").Stock);

        var outcome = service.BuyCoffee(new OrderRequest
        {
            Order = new Dictionary<string, int> { { "Cappuccino", 1 } },
            Payment = new Payment { TotalAmount = 1200 }
        });

        Assert.False(outcome.Success);
        Assert.Contains("No hay suficientes Cappuccino en la máquina.", outcome.Message);
    }
}
