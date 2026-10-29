using ContosoShop.Cart;
using ContosoShop.Catalog;

namespace ContosoShop.Tests;

[TestClass]
public sealed class ShoppingCartTests
{
    [TestMethod]
    public void NewCart_HasNoLinesAndZeroTotal()
    {
        var cart = new ShoppingCart();

        Assert.IsEmpty(cart.Lines);
        Assert.AreEqual(0m, cart.Total);
    }

    [TestMethod]
    public void Add_CombinesByProductIdAndCalculatesTotals()
    {
        var cart = new ShoppingCart();
        var products = new InMemoryProductCatalog().GetProducts();

        cart.Add(products[0]);
        cart.Add(products[1]);
        cart.Add(products[0] with { });

        Assert.HasCount(2, cart.Lines);
        Assert.AreEqual(products[0], cart.Lines[0].Product);
        Assert.AreEqual(2, cart.Lines[0].Quantity);
        Assert.AreEqual(98m, cart.Lines[0].LineTotal);
        Assert.AreEqual(products[1], cart.Lines[1].Product);
        Assert.AreEqual(1, cart.Lines[1].Quantity);
        Assert.AreEqual(24.50m, cart.Lines[1].LineTotal);
        Assert.AreEqual(122.50m, cart.Total);
    }

    [TestMethod]
    public void Add_DoesNotChangeAnotherCart()
    {
        var cart = new ShoppingCart();
        var otherCart = new ShoppingCart();

        cart.Add(new InMemoryProductCatalog().GetProducts()[0]);

        Assert.HasCount(1, cart.Lines);
        Assert.IsEmpty(otherCart.Lines);
        Assert.AreEqual(0m, otherCart.Total);
    }

    [TestMethod]
    public void Add_NullProduct_ThrowsAndLeavesCartUnchanged()
    {
        var cart = new ShoppingCart();

        Assert.Throws<ArgumentNullException>(() => cart.Add(null!));

        Assert.IsEmpty(cart.Lines);
        Assert.AreEqual(0m, cart.Total);
    }

    [TestMethod]
    public void AdjustQuantity_UsesCurrentQuantityAndPreservesOtherLines()
    {
        var cart = new ShoppingCart();
        var products = new InMemoryProductCatalog().GetProducts();
        cart.Add(products[0]);
        cart.Add(products[1]);

        cart.IncreaseQuantity(products[0].Id);
        cart.IncreaseQuantity(products[0].Id);

        Assert.AreEqual(3, cart.Lines[0].Quantity);
        Assert.AreEqual(147m, cart.Lines[0].LineTotal);
        Assert.AreEqual(171.50m, cart.Total);
        Assert.AreEqual(1, cart.Lines[1].Quantity);
        Assert.AreEqual(24.50m, cart.Lines[1].LineTotal);

        cart.DecreaseQuantity(products[0].Id);
        cart.DecreaseQuantity(products[0].Id);

        Assert.HasCount(2, cart.Lines);
        Assert.AreEqual(1, cart.Lines[0].Quantity);
        Assert.AreEqual(49m, cart.Lines[0].LineTotal);
        Assert.AreEqual(73.50m, cart.Total);
        Assert.AreEqual(products[1], cart.Lines[1].Product);
        Assert.AreEqual(1, cart.Lines[1].Quantity);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void DecreaseQuantity_AtOne_LeavesCartUnchanged(int attempts)
    {
        var cart = new ShoppingCart();
        var product = new InMemoryProductCatalog().GetProducts()[0];
        cart.Add(product);
        var originalLine = cart.Lines[0];

        for (var attempt = 0; attempt < attempts; attempt++)
        {
            cart.DecreaseQuantity(product.Id);
        }

        Assert.HasCount(1, cart.Lines);
        Assert.AreEqual(originalLine, cart.Lines[0]);
        Assert.AreEqual(49m, cart.Total);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void AdjustQuantity_UnknownProduct_ThrowsWithoutChangingCart(bool increase)
    {
        var cart = new ShoppingCart();
        cart.Add(new InMemoryProductCatalog().GetProducts()[0]);
        var originalLine = cart.Lines[0];

        Assert.Throws<KeyNotFoundException>(() =>
        {
            if (increase)
            {
                cart.IncreaseQuantity(999);
            }
            else
            {
                cart.DecreaseQuantity(999);
            }
        });

        Assert.HasCount(1, cart.Lines);
        Assert.AreEqual(originalLine, cart.Lines[0]);
        Assert.AreEqual(49m, cart.Total);
    }
}