using ContosoShop.Catalog;

namespace ContosoShop.Shopping;

public sealed record CartItem(Product Product, int Quantity)
{
    public decimal LineTotal => Product.Price * Quantity;
}