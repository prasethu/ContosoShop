using ContosoShop.Catalog;

namespace ContosoShop.Tests;

[TestClass]
public sealed class ProductCatalogTests
{
    private readonly InMemoryProductCatalog catalog = new();

    [TestMethod]
    public void GetProducts_ReturnsExpectedSeededProducts()
    {
        var products = catalog.GetProducts();

        Assert.HasCount(6, products);
        CollectionAssert.AreEquivalent(
            new[]
            {
                "Everyday Backpack",
                "Insulated Travel Mug",
                "Wireless Desk Charger",
                "Soft Knit Throw",
                "Pocket Notebook Set",
                "Trail Water Bottle"
            },
            products.Select(product => product.Name).ToArray());
    }

    [TestMethod]
    public void GetProducts_ReturnsUniqueIdsAndPositivePrices()
    {
        var products = catalog.GetProducts();

        Assert.HasCount(products.Count, products.Select(product => product.Id).Distinct());
        Assert.IsTrue(products.All(product => product.Price > 0));
        Assert.IsTrue(products.All(product => product.ImageUrl.StartsWith("/images/products/", StringComparison.Ordinal)));
        Assert.HasCount(products.Count, products.Select(product => product.ImageUrl).Distinct());
    }

    [TestMethod]
    public void GetProducts_ReturnsReadOnlyCollection()
    {
        var products = catalog.GetProducts();

        Assert.IsInstanceOfType<IList<Product>>(products);
        Assert.ThrowsExactly<NotSupportedException>(() => ((IList<Product>)products).Clear());
    }

    [TestMethod]
    public void GetProducts_ReturnsStableCatalogInstance()
    {
        var firstResult = catalog.GetProducts();
        var secondResult = catalog.GetProducts();

        Assert.AreSame(firstResult, secondResult);
    }
}
