using ContosoShop.Catalog;

namespace ContosoShop.Shopping;

public sealed class ShoppingCart
{
    private readonly Dictionary<int, CartItem> items = [];

    public IReadOnlyCollection<CartItem> Items => items.Values;

    public decimal Total => items.Values.Sum(item => item.LineTotal);

    public void Add(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        items[product.Id] = items.TryGetValue(product.Id, out var item)
            ? item with { Quantity = checked(item.Quantity + 1) }
            : new CartItem(product, 1);
    }
}