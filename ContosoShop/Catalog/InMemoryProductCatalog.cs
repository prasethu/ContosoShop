using System.Collections.ObjectModel;

namespace ContosoShop.Catalog;

public sealed class InMemoryProductCatalog : IProductCatalog
{
    private static readonly IReadOnlyList<Product> Products = new ReadOnlyCollection<Product>(
        [
            new(1, "Everyday Backpack", "A lightweight backpack for commuting and day trips.", "Bags", 49.00m, "/images/products/backpack.jpg"),
            new(2, "Insulated Travel Mug", "A leak-resistant mug that keeps drinks warm on the go.", "Drinkware", 24.50m, "/images/products/travel-mug.jpg"),
            new(3, "Wireless Desk Charger", "A compact charging pad for a tidy workspace.", "Electronics", 32.00m, "/images/products/desk-charger.jpg"),
            new(4, "Soft Knit Throw", "A comfortable throw blanket for cool evenings.", "Home", 39.95m, "/images/products/knit-throw.jpg"),
            new(5, "Pocket Notebook Set", "Three durable notebooks for plans, lists, and sketches.", "Stationery", 14.00m, "/images/products/notebooks.jpg"),
            new(6, "Trail Water Bottle", "A reusable stainless-steel bottle for daily adventures.", "Outdoors", 28.75m, "/images/products/water-bottle.jpg")
        ]);

    public IReadOnlyList<Product> GetProducts() => Products;
}
