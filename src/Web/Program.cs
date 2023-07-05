using System.Text.Json.Serialization;
using Application.AppServices.Chat;
using Application.AppServices.User;
using Application.Hubs;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Web;
using Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

builder.Configuration.AddConfiguration(configuration);

builder.Services.AddDbContext<FlitchDbContext>(c =>
    c.UseNpgsql(
        builder.Configuration["POSTGRESQLCONNSTR_ConnStr"]
    ));

builder.Services.AddFlitchAuth(builder.Configuration);
builder.Services.AddFlitchEmailing(builder.Configuration);

builder.Services.AddUsersServices();
builder.Services.AddSingleton<IUserConnectionMap<Guid>, UserConnectionMap<Guid>>();

builder.Services.AddMappings();
builder.Services.AddValidation();

builder.Services.AddControllers()
    .AddJsonOptions(
    o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwagger();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowLocalhost",
        builder =>
            builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddSignalR().AddHubOptions<MessengerHub>(options =>
{
    options.EnableDetailedErrors = true;
    options.ClientTimeoutInterval = TimeSpan.FromHours(2);
});

var app = builder.Build();

using var scope = app.Services.CreateScope();
await using var dbContext = scope.ServiceProvider.GetRequiredService<FlitchDbContext>();
await dbContext.Database.MigrateAsync();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseStaticFiles();

    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        o.InjectStylesheet("/assets/css/swagger-theme-muted.css");
    });
//}

app.UseHttpsRedirection();
app.UseCors("AllowLocalhost");

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();
app.MapHub<MessengerHub>("/chats").RequireCors("AllowLocalhost");

app.Run();
