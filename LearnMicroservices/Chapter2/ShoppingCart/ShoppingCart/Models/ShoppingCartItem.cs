namespace ShoppingCart.Models;

public record ShoppingCartItem(
    int ProductCatalogueId,
    string ProductName,
    string Description,
    Money Price)
{
    public virtual bool Equals(ShoppingCartItem? obj) =>
        obj != null
        && ProductCatalogueId.Equals(obj.ProductCatalogueId);

    public override int GetHashCode() => ProductCatalogueId.GetHashCode();
}

public record Money(
    string Currency,
    decimal Amount
);
