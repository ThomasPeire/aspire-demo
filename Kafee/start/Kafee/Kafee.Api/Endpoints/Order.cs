using Kafee.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Kafee.Api.Endpoints;

public static class OrderEndpoint
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/order");
        group.MapPost("/create", Order.Create).WithName("CreateOrder");
    }
}

public record OrderRequest(Guid Id);

internal abstract class Order
{
    public static async Task<IResult> Create(
        OrderRequest request,
        KafeeDbContext dbContext,
        CancellationToken ct)
    {
        var item = await dbContext.MenuItems.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: ct);

        if (item == null)
        {
            return Results.Problem(
                title: "Item not found",
                detail: $"Menu item with ID {request.Id} does not exist",
                statusCode: StatusCodes.Status404NotFound);
        }

        if (item.AmountInStock == 0)
        {
            return Results.Problem(
                title: "Item out of stock",
                detail: $"{item.Name} is currently out of stock",
                statusCode: StatusCodes.Status409Conflict);
        }
        
        item.PickFromStock(1);
        await dbContext.SaveChangesAsync(ct);

        return Results.Ok(new { Message = $"Ordered {item.Name}", Item = item });
    }
}