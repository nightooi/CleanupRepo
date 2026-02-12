using events.infra;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<EventsDbContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("default")));
builder.Services.AddCors(x =>
{
    x.AddPolicy("dev", x =>
    {
        x.WithOrigins("http://localhost:5173", "https://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
    

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseCors("dev");

}


app.UseRouting();
app.RegisterFeatureEndpoints();

app.Run();




