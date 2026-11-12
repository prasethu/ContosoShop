using ContosoShop.Catalog;

namespace ContosoShop.Cart;

public sealed class CartItem
{
    internal CartItem(Product product)
    {
        Product = product;
    }

    public Product Product { get; }
    public int Quantity { get; private set; } = 1;
    public decimal LineTotal => Product.Price * Quantity;

    internal void IncreaseQuantity() => Quantity = checked(Quantity + 1);
}

public sealed class ShoppingCart
{
    private readonly List<CartItem> items = [];

    public IReadOnlyList<CartItem> Items => items.AsReadOnly();
    public decimal Total => items.Sum(item => item.LineTotal);

    public void AddProduct(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);
        var existingItem = items.FirstOrDefault(item => item.Product.Id == product.Id);
        if (existingItem is not null)
        {
            existingItem.IncreaseQuantity();
        }
        else
        {
            items.Add(new CartItem(product));
        }
    }
}