namespace ProductCatalog.Data
{
    using System.Collections.Generic;
    using ProductCatalog.Models;

    public interface IProductStore
    {
        IEnumerable<ProductCatalogProduct> GetProductsByIds(IEnumerable<int> productIds);
    }
}
