namespace ShoppingCart.ShoppingCart;

using System.Net.Http.Headers;
using System.Text.Json;

public interface IProductCatalogClient
{
    Task<IEnumerable<ShoppingCartItem>> GetShoppingCartItems(int[] productCatalogIds);
}

public class ProductCatalogClient : IProductCatalogClient
{
    private readonly HttpClient _client;
    private const string _productCatalogBaseUrl = "https://git.io/JeHiE";
    private const string _getProductPathTemplate = "?productIds=[{0}]";

    public ProductCatalogClient(HttpClient client)
    {
        client.BaseAddress = new Uri(_productCatalogBaseUrl);
        client.DefaultRequestHeaders
            .Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _client = client;
    }

    public async Task<IEnumerable<ShoppingCartItem>> GetShoppingCartItems(int[] productCatalogIds)
    {
        using var response = await RequestProductFromProductCatalog(productCatalogIds);
        return await ConvertToShoppingCartItems(response);
    }

    private async Task<HttpResponseMessage> RequestProductFromProductCatalog(int[] productCatalogIds)
    {
        var productsResource = string.Format(_getProductPathTemplate, string.Join(",", productCatalogIds));
        return await _client.GetAsync(productsResource);
    }

    private static async Task<IEnumerable<ShoppingCartItem>> ConvertToShoppingCartItems(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        var products = await JsonSerializer
            .DeserializeAsync<List<ProductCatalogProduct>>(
                await response.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? [];
        return products
            .Select(p =>
                new ShoppingCartItem(
                    p.ProductId,
                    p.ProductName,
                    p.ProductDescription,
                    p.Price
                ));
    }

    private record ProductCatalogProduct(
        int ProductId,
        string ProductName,
        string ProductDescription,
        Money Price);
}
