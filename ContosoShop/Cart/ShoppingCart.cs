using ContosoShop.Catalog;

namespace ContosoShop.Cart;

/// <summary>
/// Holds selections for the current Blazor circuit; refreshes and restarts start a new cart.
/// </summary>
public sealed class ShoppingCart
{
    private readonly List<CartItem> items = [];

    public IReadOnlyList<CartItem> Items { get; }
    public decimal Total => items.Sum(item => item.LineTotal);

    public ShoppingCart()
    {
        Items = items.AsReadOnly();
    }

    public void Add(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        var item = items.SingleOrDefault(item => item.Product.Id == product.Id);
        if (item is null)
        {
            items.Add(new CartItem(product));
        }
        else
        {
            item.IncreaseQuantity();
        }
    }
}

public sealed class CartItem
{
    public Product Product { get; }
    public int Quantity { get; private set; } = 1;
    public decimal LineTotal => Product.Price * Quantity;

    internal CartItem(Product product)
    {
        Product = product;
    }

    internal void IncreaseQuantity()
    {
        Quantity = checked(Quantity + 1);
    }

    internal void DecreaseQuantity()
    {
        if (Quantity <= 1)
        {
            throw new InvalidOperationException("Cart quantity cannot be less than one.");
        }

        Quantity--;
    }
}
