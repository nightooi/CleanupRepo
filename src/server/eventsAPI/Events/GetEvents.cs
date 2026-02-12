
using events.infra;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eventsAPI.Events
{
    public class GetEvents : IEndpoint
    {
        public static void MapEndPoint(IEndpointRouteBuilder builder)
            => builder.MapGroup("events")
                .WithTags("Events")
                .MapGet("/Events", Handle)
                .WithName("GetEvents");

        public record EventDto(
            Guid id,
            string eventName,
            string eventType,
            DateTime eventStart,
            DateTime eventEnd,
            string[]? covers,
            string[]? features);

        private static async Task<IResult> Handle(
            [FromServices] EventsDbContext ctx,
            CancellationToken ct)
        {
            var events = await ctx.Events
                .Include(x => x.Features)
                .Include(x => x.Category)
                .ToListAsync(ct);

            return TypedResults.Ok(events.Select(x =>
                new EventDto(
                    x.Id,
                    x.Name,
                    x.Category?.Name ?? "default",
                    x.EventStart,
                    x.EventEnd,
                    x.Covers?.ToArray(),
                    x.Features?.Select(f => f.Name).ToArray())));
        }
    }
}
