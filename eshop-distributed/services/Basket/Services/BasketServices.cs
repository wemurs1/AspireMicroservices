
namespace Basket.Services;

public class BasketServices(IDistributedCache cache, CatalogApiClient catalogApiClient)
{
    public async Task<ShoppingCart?> GetBasket(string userName)
    {
        var basket = await cache.GetStringAsync(userName);
        return string.IsNullOrEmpty(basket) ? null : JsonSerializer.Deserialize<ShoppingCart>(basket);
    }

    public async Task UpdateBasket(ShoppingCart basket)
    {
        foreach (var item in basket.Items)
        {
            var product = await catalogApiClient.GetProductById(item.ProductId);
            item.Price = product.Price;
            item.ProductName = product.Name;
        }

        await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket));
    }

    public async Task DeleteBasket(string userName)
    {
        await cache.RemoveAsync(userName);
    }

    internal async Task UpdateBasketItemProductPrices(int productId, decimal price)
    {
        // Distributed cache does not support getting a list of keyes see - https://github.com/dotnet/runtime/issues/36402
        var basket = await GetBasket("fred");
        var item = basket!.Items.FirstOrDefault(x => x.ProductId == productId);
        if (item != null)
        {
            item.Price = price;
            await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket));
        }
    }
}
