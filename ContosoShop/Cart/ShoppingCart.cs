using ContosoShop.Catalog;

namespace ContosoShop.Cart;

public sealed class ShoppingCart
{
    private readonly List<CartItem> items = [];

    public IReadOnlyList<CartItem> Items => items.AsReadOnly();
    public decimal Total => items.Sum(item => item.LineTotal);

    public void Add(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        var item = items.Find(item => item.Product.Id == product.Id);
        if (item is null)
        {
            items.Add(new CartItem(product));
        }
        else
        {
            item.Increase();
        }
    }
}

public sealed class CartItem
{
    internal CartItem(Product product)
    {
        Product = product;
    }

    public Product Product { get; }
    public int Quantity { get; private set; } = 1;
    public decimal LineTotal => Product.Price * Quantity;

    internal void Increase() => Quantity = checked(Quantity + 1);
}
