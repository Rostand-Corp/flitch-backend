using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Web;
using Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FlitchDbContext>(c =>
    c.UseNpgsql(
        builder.Configuration["ConnectionStrings:ConnStr"])
);

builder.Services.AddFlitchAuth(builder.Configuration);
builder.Services.AddFlitchEmailing(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwagger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles();
    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        o.InjectStylesheet("/assets/css/swagger-theme-muted.css");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
