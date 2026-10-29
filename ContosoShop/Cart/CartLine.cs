using ContosoShop.Catalog;

namespace ContosoShop.Cart;

public sealed record CartLine(Product Product, int Quantity)
{
    public decimal LineTotal => Product.Price * Quantity;
}