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

        return menu.ToArray();
    }

    public async Task OrderAsync(int menuItemId, CancellationToken cancellationToken = default)
    {
        var request = new OrderRequest(menuItemId);
        var response = await httpClient.PostAsJsonAsync("/order", request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}

public record OrderRequest(int Id);

public record MenuItem(int Id, string Name, decimal Price, int AmountInStock)
{
    public bool Available => AmountInStock > 0;
}
