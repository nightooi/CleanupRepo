using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace events.infra;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers EventsDbContext for a local PostgreSQL instance.
    /// Assumes: Host=localhost, Port=5432, Database=events, Username=postgres.
    /// </summary>
    public static IServiceCollection AddLocalPostgresEventsDb(
        this IServiceCollection services,
        string password,
        string database = "events",
        string username = "postgres",
        string host = "localhost",
        int port = 5432)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password must be provided.", nameof(password));

        // Build a safe connection string (handles escaping special chars)
        var csb = new Npgsql.NpgsqlConnectionStringBuilder
        {
            Host = host,
            Port = port,
            Database = database,
            Username = username,
            Password = password,

            // Optional, but often helpful defaults for local dev:
            Pooling = true,
            Timeout = 15,
            CommandTimeout = 30
        };

        services.AddDbContext<EventsDbContext>(options =>
            options.UseNpgsql(csb.ConnectionString));

        return services;
    }
}
