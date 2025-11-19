using Azure.Messaging.ServiceBus;
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
        ServiceBusClient serviceBusClient,
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

        if (item.AmountInStock < 5)
        {
            await SendLowInStockMessage(serviceBusClient, item, ct);
        }

        return Results.Ok(new { Message = $"Ordered {item.Name}", Item = item });
    }

    private static async Task SendLowInStockMessage(ServiceBusClient serviceBusClient, MenuItem item,
        CancellationToken ct)
    {
        var sender = serviceBusClient.CreateSender("mailbrewerqueue");

        try
        {
            var serviceBusMessage =
                new ServiceBusMessage($"Item {item.Name} is low in stock. Only {item.AmountInStock} left.");
            await sender.SendMessageAsync(serviceBusMessage, ct);
        }
        finally
        {
            await sender.DisposeAsync();
        }
    }
}