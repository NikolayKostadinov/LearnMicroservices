namespace ShoppingCart.Data;

using System.Data.SqlClient;
using Dapper;
using Models;

class ShoppingCartStore : IShoppingCartStore
{
    private static readonly Dictionary<int, ShoppingCart> Database = new();

    private string _connectionString =
        @"Data Source=localhost;Initial Catalog=ShoppingCart;User Id=SA; Password=P@ssw0rd";

    private const string readItemsSql =
        @"select ShoppingCart.ID, ProductCatalogId,ProductName, ProductDescription, Currency, Amount 
            from ShoppingCart, ShoppingCartItem
            where ShoppingCartItem.ShoppingCartId = ShoppingCart.ID
            and ShoppingCart.UserId=@UserId";

    private const string insertShoppingCartSql =
        @"insert into ShoppingCart (UserId) 
            OUTPUT inserted.ID VALUES (@UserId)";


    public async Task<ShoppingCart> Get(int userId)
    {
        await using var conn = new SqlConnection(_connectionString);
        var items = (await
                conn.QueryAsync(
                    readItemsSql,
                    new { UserId = userId }))
            .ToList();
        return new ShoppingCart(
            items.FirstOrDefault()?.ID,
            userId,
            items.Select(x =>
                new ShoppingCartItem(
                    (int)x.ProductCatalogId,
                    x.ProductName,
                    x.ProductDescription,
                    new Money(x.Currency, x.Amount))));
    }


    private const string deleteAllForShoppingCartSql =
        @"delete item from ShoppingCartItem item
            inner join ShoppingCart cart 
               on item.ShoppingCartId = cart.ID and cart.UserId=@UserId";

    private const string addAllForShoppingCartSql =
        @"insert into ShoppingCartItem (ShoppingCartId, ProductCatalogId, ProductName,ProductDescription, Amount, Currency)
            values(@ShoppingCartId, @ProductCatalogId, @ProductName,@ProductDescription, @Amount, @Currency)";
    public async Task Save(ShoppingCart shoppingCart)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        await using (var tx = conn.BeginTransaction())
        {
            var shoppingCartId = shoppingCart.Id != 0
                ? shoppingCart.Id
                : await conn.QuerySingleAsync<int>(
                    insertShoppingCartSql,
                    new { shoppingCart.UserId }, tx);

            await conn.ExecuteAsync( deleteAllForShoppingCartSql, new { UserId = shoppingCart.UserId }, tx);

            var enumerable = shoppingCart.Items.Select(x =>
                new
                {
                    shoppingCartId,
                    x.ProductCatalogId,
                    Productdescription = x.Description,
                    x.ProductName,
                    x.Price.Amount,
                    x.Price.Currency
                });

            await conn.ExecuteAsync( addAllForShoppingCartSql, enumerable, tx);

            await tx.CommitAsync();
        }
    }
}
