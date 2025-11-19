namespace Kafee.Client;

public class KafeeApiClient(HttpClient httpClient)
{
    public async Task<MenuItem[]> GetMenuAsync(CancellationToken cancellationToken = default)
    {
        List<MenuItem> menu = [];

        await foreach (var item in httpClient.GetFromJsonAsAsyncEnumerable<MenuItem>("/menu", cancellationToken))
        {
            if (item is not null)
            {
                menu.Add(item);
            }
        }

        return menu.OrderBy(item => item.Name).ToArray();
    }

    public async Task OrderAsync(Guid menuItemId, CancellationToken cancellationToken = default)
    {
        var request = new OrderRequest(menuItemId);
        var response = await httpClient.PostAsJsonAsync("/order/create", request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}

public record OrderRequest(Guid Id);

public record MenuItem(Guid Id, string Name, decimal Price, int AmountInStock)
{
    public bool Available => AmountInStock > 0;
}
