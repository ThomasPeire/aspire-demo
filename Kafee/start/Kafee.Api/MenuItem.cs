namespace Kafee.Api;

public class MenuItem
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int AmountInStock { get; private set; }

    internal MenuItem()
    {
    }

    public static MenuItem Create(Guid id, string name, int amountInStock, decimal price)
    {
        return new MenuItem
        {
            Id = id,
            Name = name,
            AmountInStock = amountInStock,
            Price = price
        };
    }
    
    public void Restock(int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Restock amount must be greater than zero.");
        }

        AmountInStock += amount;
    }
    
    public void PickFromStock(int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Deduct amount must be greater than zero.");
        }

        if (AmountInStock < amount)
        {
            throw new InvalidOperationException("Insufficient stock to deduct the requested amount.");
        }
        
        AmountInStock -= amount;
    }
}