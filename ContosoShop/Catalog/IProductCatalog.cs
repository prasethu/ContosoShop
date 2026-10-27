namespace ContosoShop.Catalog;

public interface IProductCatalog
{
    IReadOnlyList<Product> GetProducts();
}
