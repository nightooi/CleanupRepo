using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using events.infra;
using Microsoft.AspNetCore.Http.HttpResults;
using events.infra.Model;
using Microsoft.EntityFrameworkCore;

namespace eventsAPI.Events
{
    public class AddEvent : IEndpoint
    {
        public static void MapEndPoint(IEndpointRouteBuilder builder)
            => builder.MapGroup("events")
            .WithTags("Events")
            .MapPost("/Events", Handle)
            .WithName("CreateEvent");
        
        public sealed record CreateEvent(
            [property: 
            Required,
            StringLength(200),
            JsonPropertyName("eventName")] string Name,
            [property: 
            Required,
            JsonPropertyName("dateStart")] DateTimeOffset EventStart,
            [property: 
            Required,
            JsonPropertyName("dateEnd")] DateTimeOffset EventEnd,
            [property: 
            JsonPropertyName("covers")] List<string>? Covers,
            [property:
            Required,
            StringLength(200),
            JsonPropertyName("eventType")] string Category,
            [property:
            JsonPropertyName("features")] List<string>? Features);


        public sealed record InvRequest<T>(string error, T value);

        [HttpPost]
        private static async Task<IResult> Handle(
            [FromBody] CreateEvent eventRequest,
            [FromServices] events.infra.EventsDbContext ctx,
            CancellationToken ct)
        {
            if (eventRequest is null)
                return TypedResults.BadRequest(new InvRequest<CreateEvent?>("no creation arguments", null));

            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(
                    eventRequest,
                    new ValidationContext(eventRequest),
                    validationResults,
                    validateAllProperties: true))
            {
                var errors = validationResults
                    .SelectMany(r =>
                        (r.MemberNames?.Any() == true ? r.MemberNames : new[] { "" })
                        .Select(m => new { Member = m, Msg = r.ErrorMessage ?? "Invalid" }))
                    .GroupBy(x => x.Member)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.Msg).ToArray());

                return TypedResults.ValidationProblem(errors);
            }

            if (eventRequest.EventEnd <= eventRequest.EventStart)
                return TypedResults.BadRequest(new InvRequest<CreateEvent>(
                    "eventEnd must be after eventStart", eventRequest));

            if (eventRequest.Covers is { Count: > 10 })
                return TypedResults.BadRequest(new InvRequest<CreateEvent>(
                    "covers may contain at most 10 items", eventRequest));

            if (eventRequest.Covers is not null && eventRequest.Covers.Any(c => c is null || c.Length > 200))
                return TypedResults.BadRequest(new InvRequest<CreateEvent>(
                    "covers contains null or an item longer than 200 characters", eventRequest));

            var featureNames = eventRequest.Features;

            var categoryName = eventRequest.Category.Trim();

            Category? category = null;
            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                category = await ctx.Categories
                    .FirstOrDefaultAsync(c => c.Name == categoryName, ct);

                if (category is null)
                {
                    category = new Category
                    {
                        Id = Guid.NewGuid(), 
                        Name = categoryName
                    };
                    ctx.Categories.Add(category);
                }
            }

            var features = new List<Feature>();
            if (featureNames.Count > 0)
            {
                var existing = await ctx.Features
                    .Where(f => featureNames.Contains(f.Name))
                    .ToListAsync(ct);

                foreach (var name in featureNames)
                {
                    var f = existing.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    if (f is null)
                    {
                        f = new Feature
                        {
                            Id = Guid.NewGuid(),
                            Name = name
                        };
                        ctx.Features.Add(f);
                    }
                    features.Add(f);
                }
            }

            var ev = new events.infra.Event
            {
                Id = Guid.NewGuid(),
                Name = eventRequest.Name,
                EventStart = eventRequest.EventStart.UtcDateTime,
                EventEnd = eventRequest.EventEnd.UtcDateTime,
                Covers = eventRequest.Covers ?? new List<string>(),

                Category = category
            };

            if (features.Count > 0)
            {
                ev.Features ??= new List<Feature>();
                foreach (var f in features)
                    ev.Features.Add(f);
            }

            ctx.Events.Add(ev);
            await ctx.SaveChangesAsync(ct);

            return TypedResults.Created($"/events/{ev.Id}", new
            {
                id = ev.Id,
                eventName = ev.Name,
                dateStart = ev.EventStart,
                dateEnd = ev.EventEnd,
                covers = ev.Covers,
                eventType = category?.Name,
                features = features.Select(x => x.Name).ToArray()
            });
        }
    }
}
