using Asp.Versioning;
using Asp.Versioning.Builder;
using Azure.Identity;
using AzureShoppingCart.Data;
using AzureShoppingCart.Data.Interceptors;
using AzureShoppingCart.Extensions;
using AzureShoppingCart.Identity;
using AzureShoppingCart.Identity.Seed;
using AzureShoppingCart.Interfaces;
using AzureShoppingCart.Middlewares;
using AzureShoppingCart.OpenApi;
using AzureShoppingCart.Options.Setups;
using AzureShoppingCart.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddScoped<AuditableInterceptor>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAntiforgery();

builder.Services.AddTransient<ILinkService, LinkService>();

builder.Services.AddScoped<IUserContext, UserContext>();

builder.Services.ConfigureOptions<SeedAdminOptionsSetup>();

builder.Services.AddDbContext<ApplicationDbContext>((sp, options) => options
    .UseSqlServer(builder.Configuration.GetConnectionString("Database"))
    .AddInterceptors(sp.GetRequiredService<AuditableInterceptor>()));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddFluentValidationAutoValidation();

ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("es");

var credential = new DefaultAzureCredential();

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(
        serviceUri: new Uri(builder.Configuration["Azure:Storage:Blob:Uri"]!))
    .WithName("Images");

    clientBuilder.AddSecretClient(vaultUri: new Uri(builder.Configuration["Azure:KeyVault:Uri"]!));

    clientBuilder.UseCredential(credential);
});

if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureKeyVault(
        vaultUri: new Uri(builder.Configuration["Azure:KeyVault:Uri"]!),
        credential: credential);
}

builder.Services.ConfigureOptions<BlobStorageSetup>();

builder.Services.AddScoped<IImageStorageService, ImageStorageService>();

builder.Services.ConfigureOptions<JwtSetup>();

builder.Services.AddScoped<ClaimsIdentityProvider>();

builder.Services.AddSingleton<TokenProvider>();

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddProblemDetails();

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddExceptionHandler<DevExceptionHandler>();
}
else
{
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
}

var app = builder.Build();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseAntiforgery();

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .HasApiVersion(new ApiVersion(2))
    .ReportApiVersions()
    .Build();

RouteGroupBuilder versionedGroup = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet)
    .AddFluentValidationAutoValidation();

app.MapEndpoints(versionedGroup);

if (app.Environment.IsDevelopment())
{
    var descriptors = app.DescribeApiVersions();

    app.MapOpenApi();

    app.MapScalarApiReference(options => options
        .WithTitle("Azure Shopping Cart API")
        .WithTheme(ScalarTheme.Kepler)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
        .DisableAgent()
        .HideClientButton()
        .AddDocuments(descriptors.Select((d, index) => new ScalarDocument(
            Name: d.GroupName,
            Title: d.GroupName,
            IsDefault: index == descriptors.Count - 1)))
        .AddPreferredSecuritySchemes(JwtBearerDefaults.AuthenticationScheme)
        .AddHttpAuthentication(JwtBearerDefaults.AuthenticationScheme, auth =>
        {
            auth.WithToken(string.Empty);
        })
        .EnablePersistentAuthentication());

    await app.ApplyMigrationsAsync();
    await app.SeedInitialDataAsync();
}

app.Run();