using ExamTwo.Models;

namespace ExamTwo.Tests;

public class CoinInventoryTests
{
    [Fact]
    public void MakeChange_EnoughCoins_UsesLargestFirst()
    {
        var inventory = new CoinInventory(new Dictionary<int, int>
        {
            { 500, 20 },
            { 100, 30 },
            { 50, 50 },
            { 25, 25 }
        });

        var result = inventory.MakeChange(650);

        Assert.True(result);

        // How many coins of each denomination should be left after making change for 650 colones
        Assert.Equal(19, inventory.Coins[500]);
        Assert.Equal(29, inventory.Coins[100]);
        Assert.Equal(49, inventory.Coins[50]);
        Assert.Equal(25, inventory.Coins[25]);
    }

    [Fact]
    public void MakeChange_NotEnoughCoins_RollsBack()
    {
        var inventory = new CoinInventory(new Dictionary<int, int>
        {
            { 500, 0 },
            { 100, 0 },
            { 50, 0 },
            { 25, 1 }
        });

        var result = inventory.MakeChange(650);

        // The result should be false because there aren't enough coins to make change for 650 colones
        Assert.False(result);
        Assert.Equal(0, inventory.Coins[500]);
        Assert.Equal(0, inventory.Coins[100]);
        Assert.Equal(0, inventory.Coins[50]);
        Assert.Equal(1, inventory.Coins[25]);
    }
}
