using ContosoShop.Catalog;

namespace ContosoShop.Shopping;

public sealed class ShoppingCart
{
    private readonly List<CartItem> items = [];

    public IReadOnlyList<CartItem> Items => items.AsReadOnly();
    public decimal Total => items.Sum(item => item.LineTotal);

    public void IncreaseQuantity(int productId) => GetItem(productId).Increment();

    public void DecreaseQuantity(int productId) => GetItem(productId).Decrement();

    private CartItem GetItem(int productId) =>
        items.Find(item => item.Product.Id == productId)
        ?? throw new KeyNotFoundException($"Product {productId} is not in the cart.");

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
            item.Increment();
        }
    }
}