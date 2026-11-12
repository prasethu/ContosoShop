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
            items[index] = items[index] with { Quantity = checked(items[index].Quantity + 1) };
        }
    }
}