namespace ContosoShop.Catalog;

public sealed record Product(
    int Id,
    string Name,
    string Description,
    string Category,
    decimal Price,
    string ImageUrl);
