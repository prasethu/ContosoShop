using ContosoShop.Catalog;

namespace ContosoShop.Cart;

public sealed class ShoppingCart
{
    private readonly List<CartLine> lines = [];

    public IReadOnlyList<CartLine> Lines { get; }
    public decimal Total => lines.Sum(line => line.LineTotal);

    public ShoppingCart()
    {
        Lines = lines.AsReadOnly();
    }

    public void IncreaseQuantity(int productId) => AdjustQuantity(productId, 1);

    public void DecreaseQuantity(int productId) => AdjustQuantity(productId, -1);

    private void AdjustQuantity(int productId, int change)
    {
        var index = lines.FindIndex(line => line.Product.Id == productId);
        if (index < 0)
        {
            throw new KeyNotFoundException($"Product {productId} is not in the cart.");
        }

        var line = lines[index];
        lines[index] = line with { Quantity = Math.Max(1, checked(line.Quantity + change)) };
    }

    public void Add(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        var index = lines.FindIndex(line => line.Product.Id == product.Id);
        if (index < 0)
        {
            lines.Add(new CartLine(product, 1));
        }
        else
        {
            var line = lines[index];
            lines[index] = line with { Quantity = checked(line.Quantity + 1) };
        }
    }
}