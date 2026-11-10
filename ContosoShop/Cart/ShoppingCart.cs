using ContosoShop.Catalog;

namespace ContosoShop.Cart;

public sealed record CartItem(Product Product, int Quantity)
{
    public decimal LineTotal => Product.Price * Quantity;
}

public sealed class ShoppingCart
{
    private readonly List<CartItem> items = [];

    public IReadOnlyList<CartItem> Items => items.AsReadOnly();
    public decimal Total => items.Sum(item => item.LineTotal);

    public void Add(Product product)
    {
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
