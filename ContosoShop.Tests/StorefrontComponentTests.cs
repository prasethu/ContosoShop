using Bunit;
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
        var linkText = component.FindAll("a")
            .Select(link => link.TextContent.Trim())
            .ToArray();

        CollectionAssert.Contains(linkText, "Home");
        CollectionAssert.Contains(linkText, "Products");
        CollectionAssert.DoesNotContain(linkText, "Counter");
        CollectionAssert.DoesNotContain(linkText, "Weather");
    }

    [TestMethod]
    public void Products_DoesNotRenderCartControls()
    {
        using var context = CreateContext();

        var component = context.Render<Products>();

        Assert.IsEmpty(component.FindAll("button"));
        Assert.IsEmpty(component.FindAll("input[type='number']"));
        Assert.IsFalse(component.Markup.Contains("order summary", StringComparison.OrdinalIgnoreCase));
    }

    private static BunitContext CreateContext()
    {
        var context = new BunitContext();
        context.Services.AddSingleton<IProductCatalog, InMemoryProductCatalog>();
        return context;
    }
}
