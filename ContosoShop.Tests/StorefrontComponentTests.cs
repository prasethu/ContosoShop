using Bunit;
using ContosoShop.Cart;
using ContosoShop.Catalog;
using ContosoShop.Components.Layout;
using ContosoShop.Components.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace ContosoShop.Tests;

[TestClass]
public sealed class StorefrontComponentTests
{
    [TestMethod]
    public void Products_RendersEverySeededProduct()
    {
        using var context = CreateContext();
        var products = new InMemoryProductCatalog().GetProducts();

        var component = context.Render<Products>();

        Assert.HasCount(products.Count, component.FindAll(".product-card"));

        foreach (var product in products)
        {
            StringAssert.Contains(component.Markup, product.Name);
            StringAssert.Contains(component.Markup, product.Category);
            StringAssert.Contains(component.Markup, product.Price.ToString("C"));
            Assert.AreEqual(product.ImageUrl, component.Find($"img[alt='{product.Name}']").GetAttribute("src"));
        }
    }

    [TestMethod]
    public void Home_ProvidesProductsCallToAction()
    {
        using var context = CreateContext();

        var component = context.Render<Home>();
        var link = component.Find("a[href='products']");

        StringAssert.Contains(link.TextContent, "Browse products");
    }

    [TestMethod]
    public void Navigation_ContainsOnlyStorefrontDestinations()
    {
        using var context = CreateContext();

        var component = context.Render<NavMenu>();
        var brand = component.Find("a.brand");
        var linkText = component.FindAll("a")
            .Select(link => link.TextContent.Trim())
            .ToArray();

        Assert.AreEqual("Contoso Store home", brand.GetAttribute("aria-label"));
        StringAssert.Contains(brand.TextContent, "Contoso Store");
        CollectionAssert.Contains(linkText, "Home");
        CollectionAssert.Contains(linkText, "Products");
        CollectionAssert.Contains(linkText, "Cart");
        CollectionAssert.DoesNotContain(linkText, "Counter");
        CollectionAssert.DoesNotContain(linkText, "Weather");
    }

    [TestMethod]
    public void Products_AddsRepeatedProductToSingleCartLine()
    {
        using var context = CreateContext();

        var component = context.Render<Products>();
        var buttons = component.FindAll("button");

        Assert.HasCount(6, buttons);
        buttons[0].Click();
        component.FindAll("button")[0].Click();

        var cart = context.Services.GetRequiredService<ShoppingCart>();
        var item = Assert.ContainsSingle(cart.Items);
        Assert.AreEqual("Everyday Backpack", item.Product.Name);
        Assert.AreEqual(2, item.Quantity);
        Assert.AreEqual(98m, item.LineTotal);
        Assert.AreEqual(98m, cart.Total);
        Assert.AreEqual("Everyday Backpack added to cart.", component.Find("[role='status']").TextContent);
        Assert.IsEmpty(component.FindAll("input[type='number']"));
    }

    [TestMethod]
    public void Cart_Empty_ShowsContinueShoppingAndZeroTotal()
    {
        using var context = CreateContext();

        var component = context.Render<ContosoShop.Components.Pages.Cart>();

        Assert.AreEqual("Your cart is empty.", component.Find(".cart-empty").TextContent);
        Assert.AreEqual("Continue shopping", component.Find("a[href='products']").TextContent);
        Assert.AreEqual($"Cart total: {0m.ToString("C")}", component.Find(".cart-total").TextContent);
        Assert.IsEmpty(component.FindAll("tbody tr"));
    }

    [TestMethod]
    public void Cart_AfterProductPage_ShowsSelectionsAndExactTotals()
    {
        using var context = CreateContext();
        var products = context.Render<Products>();
        products.FindAll("button")[0].Click();
        products.FindAll("button")[1].Click();
        products.FindAll("button")[1].Click();
        products.Dispose();

        var component = context.Render<ContosoShop.Components.Pages.Cart>();
        var rows = component.FindAll("tbody tr");

        Assert.HasCount(2, rows);
        CollectionAssert.AreEqual(
            new[] { "Everyday Backpack", 49m.ToString("C"), "1", 49m.ToString("C") },
            rows[0].QuerySelectorAll("th, td:not(.quantity-cell), .cart-quantity").Select(cell => cell.TextContent).ToArray());
        CollectionAssert.AreEqual(
            new[] { "Insulated Travel Mug", 24.5m.ToString("C"), "2", 49m.ToString("C") },
            rows[1].QuerySelectorAll("th, td:not(.quantity-cell), .cart-quantity").Select(cell => cell.TextContent).ToArray());
        Assert.AreEqual($"Cart total: {98m.ToString("C")}", component.Find(".cart-total").TextContent);
        Assert.IsEmpty(component.FindAll(".cart-empty"));
    }

    [TestMethod]
    public void Cart_QuantityControls_RecalculateTotalsAndDisableDecreaseAtOne()
    {
        using var context = CreateContext();
        var cart = context.Services.GetRequiredService<ShoppingCart>();
        var products = new InMemoryProductCatalog().GetProducts();
        cart.Add(products[0]);
        cart.Add(products[1]);
        var component = context.Render<ContosoShop.Components.Pages.Cart>();

        Assert.IsTrue(component.Find("button[aria-label='Decrease Everyday Backpack quantity']").HasAttribute("disabled"));
        component.Find("button[aria-label='Increase Everyday Backpack quantity']").Click();

        Assert.AreEqual("2", component.FindAll(".cart-quantity")[0].TextContent);
        Assert.AreEqual(98m.ToString("C"), component.FindAll("tbody tr")[0].Children[3].TextContent);
        Assert.AreEqual($"Cart total: {122.5m.ToString("C")}", component.Find(".cart-total").TextContent);
        Assert.IsFalse(component.Find("button[aria-label='Decrease Everyday Backpack quantity']").HasAttribute("disabled"));
        Assert.AreEqual("1", component.FindAll(".cart-quantity")[1].TextContent);
        Assert.AreEqual(24.5m.ToString("C"), component.FindAll("tbody tr")[1].Children[3].TextContent);

        component.Find("button[aria-label='Decrease Everyday Backpack quantity']").Click();

        Assert.AreEqual("1", component.FindAll(".cart-quantity")[0].TextContent);
        Assert.AreEqual(49m.ToString("C"), component.FindAll("tbody tr")[0].Children[3].TextContent);
        Assert.AreEqual($"Cart total: {73.5m.ToString("C")}", component.Find(".cart-total").TextContent);
        Assert.IsTrue(component.Find("button[aria-label='Decrease Everyday Backpack quantity']").HasAttribute("disabled"));
        Assert.HasCount(2, cart.Items);
        Assert.AreEqual(1, cart.Items[0].Quantity);
        Assert.AreEqual(1, cart.Items[1].Quantity);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    public void ShoppingCart_SetQuantityBelowOne_RejectsChange(int quantity)
    {
        var cart = new ShoppingCart();
        cart.Add(new InMemoryProductCatalog().GetProducts()[0]);

        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => cart.SetQuantity(1, quantity));

        Assert.AreEqual("quantity", exception.ParamName);
        Assert.AreEqual(1, Assert.ContainsSingle(cart.Items).Quantity);
        Assert.AreEqual(49m, cart.Total);
    }

    private static BunitContext CreateContext()
    {
        var context = new BunitContext();
        context.Services.AddSingleton<IProductCatalog, InMemoryProductCatalog>();
        context.Services.AddScoped<ShoppingCart>();
        return context;
    }
}
