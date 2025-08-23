using XPE.ArquiteturaSoftware.DesafioFinal.Domain.Models;

namespace XPE.ArquiteturaSoftware.DesafioFinal.Infra.Data.Queries;

public static class ProductQueries
{
    public const string Create = """
        INSERT INTO products
            (name, description, price, active, created_at)
        VALUES
            (@Name, @Description, @Price, @Active, NOW());
        SELECT CAST(LAST_INSERT_ID() AS SIGNED);
    """;

    public const string GetById = $"""
        {GetBase}
        WHERE id = @Id;
    """;

    public const string GetAll = $"""
        {GetBase}
        ORDER BY id DESC;
    """;

    public const string FindByName = $"""
        {GetBase}
        WHERE name LIKE @Pattern
        ORDER BY id DESC;
    """;

    private const string GetBase = $"""
        SELECT 
            id          AS {nameof(Product.Id)}, 
            name        AS {nameof(Product.Name)}, 
            description AS {nameof(Product.Description)}, 
            price       AS {nameof(Product.Price)}, 
            active      AS {nameof(Product.Active)}, 
            created_at  AS {nameof(Product.CreatedAt)}, 
            updated_at  AS {nameof(Product.UpdatedAt)}
        FROM products
    """;

    public const string Count = "SELECT COUNT(1) FROM products;";

    public const string Update = """
        UPDATE products
        SET 
            name = @Name,
            description = @Description,
            price = @Price,
            active = @Active,
            updated_at = NOW()
        WHERE id = @Id;
    """;

    public const string Delete = """
        DELETE FROM products
        WHERE id = @Id;
    """;
}