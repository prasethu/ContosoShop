using Bunit;
using ContosoShop.Catalog;
using ContosoShop.Cart;
using ContosoShop.Components.Layout;
using ContosoShop.Components.Pages;
using Microsoft.Extensions.DependencyInjection;
using CartPage = ContosoShop.Components.Pages.Cart;

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
        var linkText = component.FindAll("a")
            .Select(link => link.TextContent.Trim())
            .ToArray();

        CollectionAssert.Contains(linkText, "Home");
        CollectionAssert.Contains(linkText, "Products");
        CollectionAssert.Contains(linkText, "Cart");
        CollectionAssert.DoesNotContain(linkText, "Counter");
        CollectionAssert.DoesNotContain(linkText, "Weather");
    }

    [TestMethod]
    public void Products_AddingProducts_ShowsCombinedQuantitiesAndTotalsInCart()
    {
        using var context = CreateContext();
        var products = context.Render<Products>();

        Assert.HasCount(6, products.FindAll("button"));
        products.Find("button[aria-label='Add Everyday Backpack to cart']").Click();
        products.Find("button[aria-label='Add Everyday Backpack to cart']").Click();
        products.Find("button[aria-label='Add Insulated Travel Mug to cart']").Click();
        Assert.AreEqual("Added Insulated Travel Mug to your cart.", products.Find("[role='status']").TextContent);
        products.Dispose();
        using var home = context.Render<Home>();

        var cart = context.Render<CartPage>();
        var rows = cart.FindAll("tbody tr");
        Assert.HasCount(2, rows);
        CollectionAssert.AreEqual(
            new[] { "Everyday Backpack", 49m.ToString("C"), "2", 98m.ToString("C") },
            rows[0].Children.Select(cell => cell.TextContent.Trim()).ToArray());
        CollectionAssert.AreEqual(
            new[] { "Insulated Travel Mug", 24.50m.ToString("C"), "1", 24.50m.ToString("C") },
            rows[1].Children.Select(cell => cell.TextContent.Trim()).ToArray());
        Assert.AreEqual(122.50m.ToString("C"), cart.Find(".cart-total strong").TextContent);
    }

    [TestMethod]
    public void Cart_WhenEmpty_ShowsContinueShoppingLink()
    {
        using var context = CreateContext();

        var cart = context.Render<CartPage>();

        StringAssert.Contains(cart.Markup, "Your cart is empty.");
        Assert.AreEqual("Continue shopping", cart.Find("a[href='products']").TextContent);
        Assert.IsEmpty(cart.FindAll("tbody tr"));
    }

    private static BunitContext CreateContext()
    {
        var context = new BunitContext();
        context.Services.AddSingleton<IProductCatalog, InMemoryProductCatalog>();
        context.Services.AddScoped<ShoppingCart>();
        return context;
    }
}
