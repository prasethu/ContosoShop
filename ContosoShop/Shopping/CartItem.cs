using ContosoShop.Catalog;

namespace ContosoShop.Shopping;

public sealed class CartItem(Product product)
{
    public Product Product { get; } = product;
    public int Quantity { get; private set; } = 1;
    public decimal LineTotal => Product.Price * Quantity;

    public void Increase() => Quantity = checked(Quantity + 1);

    public void Decrease()
    {
        if (Quantity <= 1)
        {
            throw new InvalidOperationException("Cart quantity cannot be less than one.");
        }

        Quantity--;
    }
}