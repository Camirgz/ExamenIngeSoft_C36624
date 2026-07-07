using ExamTwo.Controllers;

namespace ExamTwo.Tests;

public class DatabaseTests
{
    [Fact]
    public void InitialCoffeeStockTest()
    {
        var db = new Database();

        Assert.Equal(10, db.coffeeStock["Americano"]);
        Assert.Equal(8, db.coffeeStock["Cappuccino"]);
        Assert.Equal(10, db.coffeeStock["Lates"]);
        Assert.Equal(15, db.coffeeStock["Mocaccino"]);
    }


    
}
