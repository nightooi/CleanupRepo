using System.Runtime.CompilerServices;
using eventsAPI.Events;

public static class RegisterEndpoints
{

    public static IEndpointRouteBuilder RegisterFeatureEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.Map<AddEvent>();
        builder.Map<GetEvents>();
        return builder;
    }
    public static IEndpointRouteBuilder Map<TEndPoint>(this IEndpointRouteBuilder config) where TEndPoint : IEndpoint
    {
        TEndPoint.MapEndPoint(config);
        return config;
    }
}




