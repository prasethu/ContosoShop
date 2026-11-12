using ContosoShop.Catalog;

namespace ContosoShop.Cart;

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

    public void SetQuantity(int productId, int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(quantity, 1);

        items[productId] = items[productId] with { Quantity = quantity };
    }
}

public sealed record CartItem(Product Product, int Quantity)
{
    public decimal LineTotal => Product.Price * Quantity;
}