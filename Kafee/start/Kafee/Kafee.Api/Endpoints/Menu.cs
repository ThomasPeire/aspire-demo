using Kafee.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Kafee.Api.Endpoints;

public static class MenuEndpoint
{
    public static void MapMenuEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/menu");
        group.MapGet("/", Menu.Get).WithName("GetMenu");
    }
}

public record MenuItemDto(Guid Id, string Name, decimal Price, int AmountInStock)
{
    public static MenuItemDto FromMenuItem(MenuItem item) => new(item.Id, item.Name, item.Price, item.AmountInStock);
}

internal static class Menu
{
    internal static async Task<IResult> Get(KafeeDbContext dbContext, CancellationToken ct)
    {
        return Results.Ok(await dbContext.MenuItems
            .AsNoTracking().Select(m => MenuItemDto.FromMenuItem(m)).ToArrayAsync(ct));
    }
}