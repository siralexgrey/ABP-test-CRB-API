using Microsoft.EntityFrameworkCore;
using ConferenceRooms.Api.Data;
using ConferenceRooms.Api.Repositories;
using ConferenceRooms.Api.Services;
using ConferenceRooms.Api.Domain.Pricing;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default");

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddSchemaTransformer<IntSchemaFixTransformer>();
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(connectionString);
});

// repos
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// services
builder.Services.AddScoped<IRoomAppService, RoomAppService>();
builder.Services.AddScoped<IBookingAppService, BookingAppService>();
builder.Services.AddScoped<IReportAppService, ReportAppService>();

builder.Services.AddSingleton<IPricingService, PricingService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    DbSeeder.Seed(db);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "openapi/v1.json";
    });
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
