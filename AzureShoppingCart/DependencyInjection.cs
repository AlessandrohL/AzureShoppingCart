using Asp.Versioning;
using Azure.Identity;
using AzureShoppingCart.Data;
using AzureShoppingCart.Data.Interceptors;
using AzureShoppingCart.Extensions;
using AzureShoppingCart.Identity;
using AzureShoppingCart.Identity.Seed;
using AzureShoppingCart.Interfaces;
using AzureShoppingCart.Middlewares;
using AzureShoppingCart.OpenApi;
using AzureShoppingCart.Options;
using AzureShoppingCart.Options.Setups;
using AzureShoppingCart.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace AzureShoppingCart;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddApiServices(this WebApplicationBuilder builder)
    {
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict;
        });

        builder.Services
            .AddOpenApi("v1", options => options
                .AddScalarTransformers()
                .AddDocumentTransformer<BearerSecuritySchemeTransformer>())
            .AddOpenApi("v2", options => options
                .AddScalarTransformers()
                .AddDocumentTransformer<BearerSecuritySchemeTransformer>())
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1.0);
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

        return builder;
    }

    public static WebApplicationBuilder AddErrorHandling(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails();

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddExceptionHandler<DevExceptionHandler>();
        }
        else
        {
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        }

        return builder;
    }

    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) => options
            .UseSqlServer(builder.Configuration.GetConnectionString("Database"))
            .AddInterceptors(sp.GetRequiredService<AuditableInterceptor>()));

        return builder;
    }

    public static WebApplicationBuilder AddAzureServices(this WebApplicationBuilder builder)
    {
        builder.Services.ConfigureOptions<BlobStorageSetup>();

        BlobStorageOptions blobStorageOptions = builder.Configuration
            .GetSection("Azure:Storage:Blob")
            .Get<BlobStorageOptions>()!;

        var credential = new DefaultAzureCredential();

        builder.Services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(
                serviceUri: new Uri(blobStorageOptions.Uri))
            .WithName("Images");

            clientBuilder.AddSecretClient(vaultUri: new Uri(blobStorageOptions.Uri));

            clientBuilder.UseCredential(credential);
        });

        if (builder.Environment.IsProduction())
        {
            builder.Configuration.AddAzureKeyVault(
                vaultUri: new Uri(builder.Configuration["Azure:KeyVault:Uri"]!),
                credential: credential);
        }

        return builder;
    }

    public static WebApplicationBuilder AddAuthenticationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.ConfigureOptions<JwtSetup>();

        JwtOptions jwtAuthOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthOptions.SecretKey)),
                    ValidIssuer = jwtAuthOptions.Issuer,
                    ValidAudience = jwtAuthOptions.Audience,
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddAuthorization();

        return builder;
    }

    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(TimeProvider.System);

        builder.Services.AddScoped<AuditableInterceptor>();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddAntiforgery();

        builder.Services.AddTransient<ILinkService, LinkService>();

        builder.Services.AddScoped<IUserContext, UserContext>();

        builder.Services.ConfigureOptions<SeedAdminOptionsSetup>();

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        builder.Services.AddFluentValidationAutoValidation();

        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("es");

        builder.Services.AddScoped<IImageStorageService, ImageStorageService>();

        builder.Services.AddScoped<ClaimsIdentityProvider>();

        builder.Services.AddSingleton<TokenProvider>();

        return builder;
    }
}
