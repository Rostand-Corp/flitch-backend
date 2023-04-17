using System.Reflection;
using System.Text;
using Application.Chats.Mappings;
using Domain.Validators;
using FluentValidation;
using Infrastructure.Auth;
using Infrastructure.Data;
using Infrastructure.Services;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Web;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFlitchAuth(this IServiceCollection services, IConfiguration config)
    {
        services.AddIdentity<SystemUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<FlitchDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = config["Auth:AllowedUserNameCharacters"]!;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.SignIn.RequireConfirmedPhoneNumber = false;
            options.SignIn.RequireConfirmedEmail = false;
        });

        services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = config["JWT:ValidAudience"],
                    ValidIssuer = config["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Secret"]!))
                };
            });
        services.AddAuthorization(o =>
        {
            o.AddPolicy("MessengerId", policy => policy.RequireClaim("msngrUserId"));
        });
        services.AddTransient<IAuthManager, AuthManager>();
        return services;
    }

    public static IServiceCollection AddFlitchEmailing(this IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<IEmailSender, SendGridEmailSender>();
        services.Configure<AuthMessageSenderOptions>(o => { o.SendGridKey = config["Email:SendGridKey"]; });
        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(o =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description =
                    "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\""
            });
            o.MapType<ProblemDetails>(() => new OpenApiSchema
            {
                Type = "object",
                Properties =
                {
                    {"type", new OpenApiSchema {Type = "string"}},
                    {"title", new OpenApiSchema {Type = "string"}},
                    {"status", new OpenApiSchema {Type = "integer", Format = "int32"}},
                    {"detail", new OpenApiSchema {Type = "string"}},
                    {"traceId", new OpenApiSchema{Type = "string"}},
                },
            });
            o.MapType<ValidationProblemDetails>(() => new OpenApiSchema
            {
                Type = "object",
                Properties =
                {
                    {"type", new OpenApiSchema {Type = "string"}},
                    {"title", new OpenApiSchema {Type = "string"}},
                    {"status", new OpenApiSchema {Type = "integer", Format = "int32"}},
                    {"detail", new OpenApiSchema {Type = "string"}},
                    {"traceId", new OpenApiSchema {Type = "string"}},
                    {"errors", new OpenApiSchema
                        {
                            Type = "object",
                            AdditionalPropertiesAllowed = true,
                            AdditionalProperties = new OpenApiSchema
                            {
                                Type = "array",
                                Items = new OpenApiSchema { Type = "string" }
                            }
                        }
                    }
                },
            });
                o.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });
        return services;
    }

    public static IServiceCollection AddMappings(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly(),
            typeof(ChatMappingConfig).Assembly); // change later to some marker mayve

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }

    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<UserValidator>(ServiceLifetime.Transient); // change to assembly name

        return services;
    }
}
