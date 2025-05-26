using DigitalBank.Application.Common;
using DigitalBank.Infrastructure.Exceptions;
using DigitalBank.Infrastructure.Persistence;
using DigitalBank.Infrastructure.Swagger;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace DigitalBank;

public static class ConfigureServices
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.AddDigitalBank();
        builder.AddSwagger();
        builder.AddCors();
        builder.AddDatabase();
        builder.AddJwtAuthentication();
    }

    private static void AddDigitalBank(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
    }

    private static void AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "DigitalBank API",
                Version = "v1",
                Description = "API for managing banking operations"
            });

            options.SchemaFilter<EnumSchemaFilter>();

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    Array.Empty<string>()
                }
            });

            options.SchemaGeneratorOptions.SupportNonNullableReferenceTypes = false;
            options.CustomSchemaIds(type => type.Name);
            options.InferSecuritySchemes();
        });
    }

    private static void AddCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins("http://localhost:4200", "http://localhost:5173", "http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });
    }

    private static void AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<DigitalBankDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });
    }

    private static void AddJwtAuthentication(this WebApplicationBuilder builder)
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);
        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                // Custom token retrieval from cookie
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // Extract token from authToken cookie
                        var token = context.Request.Cookies["authToken"];
                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        builder.Services.AddAuthorization();
    }
}
