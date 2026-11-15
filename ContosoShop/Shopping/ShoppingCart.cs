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

        var item = items.Find(item => item.Product.Id == product.Id);
        if (item is null)
        {
            items.Add(new CartItem(product));
        }
        else
        {
            item.Quantity = checked(item.Quantity + 1);
        }
    }

    public void IncreaseQuantity(int productId)
    {
        var item = items.Single(item => item.Product.Id == productId);
        item.Quantity = checked(item.Quantity + 1);
    }

    public void DecreaseQuantity(int productId)
    {
        var item = items.Single(item => item.Product.Id == productId);
        item.Quantity = Math.Max(1, item.Quantity - 1);
    }
}