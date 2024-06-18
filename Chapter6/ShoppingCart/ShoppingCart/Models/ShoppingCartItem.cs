namespace ShoppingCart.Models;

public record ShoppingCartItem(
    int ProductCatalogId,
    string ProductName,
    string Description,
    Money Price)
{
    public virtual bool Equals(ShoppingCartItem? obj) =>
        obj != null
        && ProductCatalogId.Equals(obj.ProductCatalogId);

    public override int GetHashCode() => ProductCatalogId.GetHashCode();
}

public record Money(
    string Currency,
    decimal Amount
);
