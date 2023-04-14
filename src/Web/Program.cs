using Application.Chats.Services;
using Application.Services;
using Application.Users.Services;
using Infrastructure.Data;
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

foreach (var value in configuration.AsEnumerable())
{
    Console.WriteLine($"{value.Key} {value.Value}");
}
Console.WriteLine(Environment.GetEnvironmentVariable("POSTGRESQLCONNSTR_ConnStr"));

builder.Services.AddDbContext<FlitchDbContext>(c =>
    c.UseNpgsql(
        builder.Configuration["POSTGRESQLCONNSTR_ConnStr"])
);

builder.Services.AddFlitchAuth(builder.Configuration);
builder.Services.AddFlitchEmailing(builder.Configuration);

builder.Services.AddTransient<ICurrentUserService, CurrentUserService>();
builder.Services.AddTransient<IUserAppService, UserAppService>();
builder.Services.AddTransient<IChatService, ChatService>();
builder.Services.AddMappings();
builder.Services.AddValidation();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwagger();

var app = builder.Build();

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

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
