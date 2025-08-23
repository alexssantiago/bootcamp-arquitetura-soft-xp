namespace XPE.ArquiteturaSoftware.DesafioFinal.Domain.Models;

public sealed class Product
{
    public int Id { get; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public bool Active { get; private set; }
    public DateTime CreatedAt { get; } = DateTime.Now;
    public DateTime? UpdatedAt { get; private set; }

    public Product(string name, string description, decimal price)
    {
        Name = name;
        Description = description;
        Price = price;
    }

    public Product() { }

    public void SetActive(bool active = true)
    {
        Active = active;
        UpdatedAt = DateTime.Now;
    }
}