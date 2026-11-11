using ContosoShop.Catalog;

namespace ContosoShop.Shopping;

public sealed class CartItem(Product product)
{
    public Product Product { get; } = product;
    public int Quantity { get; private set; } = 1;
    public decimal LineTotal => Product.Price * Quantity;

    internal void Increase() => Quantity = checked(Quantity + 1);
}