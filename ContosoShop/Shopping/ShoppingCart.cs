using ContosoShop.Catalog;

namespace ContosoShop.Shopping;

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

        var index = items.FindIndex(item => item.Product.Id == product.Id);
        if (index < 0)
        {
            items.Add(new CartItem(product, 1));
        }
        else
        {
            var item = items[index];
            items[index] = item with { Quantity = checked(item.Quantity + 1) };
        }
    }
}