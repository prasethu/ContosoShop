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

    internal void IncreaseQuantity() => SetQuantity(checked(Quantity + 1));

    internal void SetQuantity(int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(quantity, 1);
        Quantity = quantity;
    }
}

public sealed class ShoppingCart
{
    private readonly List<CartItem> items = [];

    public ShoppingCart()
    {
        Items = items.AsReadOnly();
    }

    public IReadOnlyList<CartItem> Items { get; }
    public decimal Total => items.Sum(item => item.LineTotal);

    public void SetQuantity(int productId, int quantity)
    {
        var item = items.Find(item => item.Product.Id == productId)
            ?? throw new KeyNotFoundException($"Product {productId} is not in the cart.");
        item.SetQuantity(quantity);
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
            item.IncreaseQuantity();
        }
    }
}