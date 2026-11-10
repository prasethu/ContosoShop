using ContosoShop.Catalog;

namespace ContosoShop.Shopping;

public sealed class ShoppingCart
{
    private readonly List<CartItem> items = [];

    public IReadOnlyList<CartItem> Items => items.AsReadOnly();
    public decimal Total => items.Sum(item => item.LineTotal);

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

    public void SetQuantity(int productId, int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(quantity, 1);

        var index = items.FindIndex(item => item.Product.Id == productId);
        if (index < 0)
        {
            throw new KeyNotFoundException($"Product {productId} is not in the cart.");
        }

        items[index] = items[index] with { Quantity = quantity };
    }
}

public sealed record CartItem(Product Product, int Quantity)
{
    public decimal LineTotal => Product.Price * Quantity;
}