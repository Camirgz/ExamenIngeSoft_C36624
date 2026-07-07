namespace ExamTwo.Models
{
    public class CoffeeType
    {
        public string Name { get; }
        public int Price { get; }
        public int Stock { get; private set; }

        public CoffeeType(string name, int price, int stock)
        {
            Name = name;
            Price = price;
            Stock = stock;
        }

        public bool hasStock(int quantity) => quantity <= Stock;

        public void Deduct(int quantity)
        {
            if (!hasStock(quantity))
                throw new InvalidOperationException($"No hay suficientes {Name} en la máquina.");

            Stock -= quantity;
        }
    }
}
