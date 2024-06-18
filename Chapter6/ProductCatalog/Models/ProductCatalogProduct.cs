namespace ProductCatalog.Models
{
    public record ProductCatalogProduct(int ProductId, string ProductName, string Description, Money Price);
}
