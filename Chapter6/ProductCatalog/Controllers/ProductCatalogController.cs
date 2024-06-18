namespace ProductCatalog.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    [Route("/products")]
    public class ProductCatalogController : ControllerBase
    {
        private readonly IProductStore _productStore;

        public ProductCatalogController(IProductStore productStore) => _productStore = productStore;

        private const int MAX_AGE = 24 * 60 * 60; // a day in seconds

        [HttpGet("")]
        [ResponseCache(Duration = MAX_AGE)]
        public IEnumerable<ProductCatalogProduct> Get([FromQuery] string productIds)
        {
            var products = _productStore.GetProductsByIds(ParseProductIdsFromQueryString(productIds));
            return products;
        }

        private static IEnumerable<int> ParseProductIdsFromQueryString(string productIdsString) =>
            productIdsString.Split(',')
                .Select(s => s.Replace("[", "").Replace("]", ""))
                .Select(int.Parse);
    }
}
