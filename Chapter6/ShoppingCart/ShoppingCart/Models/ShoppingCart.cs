namespace ShoppingCart.Models;

using Data;

public class ShoppingCart
{
    private readonly HashSet<ShoppingCartItem> _items = new();
    public int Id { get; set; }
    public int UserId { get; }
    public IEnumerable<ShoppingCartItem> Items => _items;

    public ShoppingCart(int userId) => UserId = userId;
    public ShoppingCart(int id, int userId, IEnumerable<ShoppingCartItem> items )
    {
        Id = id;
        UserId = userId;
        _items = [..items];
    }

    public void AddItems(IEnumerable<ShoppingCartItem> shoppingCartItems, IEventStore eventStore)
    {
        foreach (var item in shoppingCartItems)
        {
            if (_items.Add(item))
            {
                eventStore.Raise("ShoppingCartItemAdded", new { UserId, item });
            }
        }
    }

    public void RemoveItems(int[] productCatalogueIds, IEventStore eventStore)
    {
        var itemsToRemove = _items.Where(i => productCatalogueIds.Contains(i.ProductCatalogId));

        foreach (var item in itemsToRemove)
        {
            if (_items.Remove(item))
            {
                eventStore.Raise("ShoppingCartItemDeleted", new { UserId, item });
            }
        }
    }
}
