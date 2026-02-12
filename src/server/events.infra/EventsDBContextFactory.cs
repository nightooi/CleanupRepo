using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using events.infra;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

public sealed class EventsDbContextFactory : IDesignTimeDbContextFactory<EventsDbContext>
{
    public EventsDbContext CreateDbContext(string[] args)
    {
        // Use env var so you don't hardcode secrets
        var cs = "Host=localhost;Port=5432;Database=events;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<EventsDbContext>()
            .UseNpgsql(cs)
            .Options;

        return new EventsDbContext(options);
    }
}
