namespace ProductCatalog.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using ProductCatalog.Models;

    public class ProductStore : IProductStore
    {
        public IEnumerable<ProductCatalogProduct> GetProductsByIds(IEnumerable<int> productIds) =>
            productIds.Select(id => new ProductCatalogProduct(id, "foo" + id, "bar", new Money()));
    }
}
