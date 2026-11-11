using Bunit;
using ContosoShop.Catalog;
using ContosoShop.Components.Layout;
using ContosoShop.Components.Pages;
using ContosoShop.Shopping;
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
        CollectionAssert.DoesNotContain(linkText, "Counter");
        CollectionAssert.DoesNotContain(linkText, "Weather");
    }

    [TestMethod]
    public void Products_ProvidesAddToCartForEveryProduct()
    {
        using var context = CreateContext();

        var component = context.Render<Products>();

        foreach (var product in new InMemoryProductCatalog().GetProducts())
        {
            var button = component.Find($"button[aria-label='Add {product.Name} to cart']");
            Assert.AreEqual("Add to cart", button.TextContent.Trim());
        }
    }

    private static BunitContext CreateContext()
    {
        var context = new BunitContext();
        context.Services.AddSingleton<IProductCatalog, InMemoryProductCatalog>();
        context.Services.AddScoped<ShoppingCart>();
        return context;
    }
}
