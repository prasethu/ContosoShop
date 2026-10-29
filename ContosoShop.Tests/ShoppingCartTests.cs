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
}