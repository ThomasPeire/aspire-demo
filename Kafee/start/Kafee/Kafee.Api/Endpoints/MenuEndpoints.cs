namespace Kafee.Api.Endpoints;

public record OrderRequest(int Id);
public record MenuItem(int Id, string Name, decimal Price, int AmountInStock);

public static class MenuEndpoints
{
    //todo move to database
    private static readonly MenuItem[] Menu =
    [
        new(1, "Cola", 2.80m, 20),
        new(2, "Jupiler", 3.00m, 14),
        new(3, "Picon", 5.50m, 2),
        new(4, "Gin-Tonic", 9.50m, 0)
    ];

    public static void MapMenuEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/");

        group.MapGet("/menu", GetMenu).WithName("GetMenu");
        group.MapPost("/order", CreateOrder).WithName("CreateOrder");    }

    private static IResult GetMenu()
    {
        return Results.Ok(Menu);
    }

    private static IResult CreateOrder(OrderRequest request)
    {
        var item = Menu.FirstOrDefault(x => x.Id == request.Id);

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

        return Results.Ok(new { Message = $"Ordered {item.Name}", Item = item });
    }
}