using ContosoShop.Catalog;

namespace ContosoShop.Shopping;

public sealed class CartItem(Product product)
{
    public Product Product { get; } = product;
    public int Quantity { get; private set; } = 1;
    public decimal LineTotal => Product.Price * Quantity;

    internal void Increment() => Quantity = checked(Quantity + 1);

    internal void Decrement()
    {
        if (Quantity <= 1)
        {
            throw new InvalidOperationException("Cart quantities cannot fall below one.");
        }

        Quantity--;
    }
}