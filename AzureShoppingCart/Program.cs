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
using AzureShoppingCart.Options.Setups;
using AzureShoppingCart.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Scalar.AspNetCore;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict;
});

builder.Services
    .AddOpenApi("v1", options => options.AddScalarTransformers())
    .AddOpenApi("v2", options => options.AddScalarTransformers())
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

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(
        serviceUri: new Uri(builder.Configuration["Azure:Storage:Blob:Uri"]!))
    .WithName("Images");

    var credential = new ChainedTokenCredential(
        new AzureCliCredential(),
        new ManagedIdentityCredential(ManagedIdentityId.SystemAssigned));

    clientBuilder.UseCredential(credential);
});

builder.Services.ConfigureOptions<BlobStorageSetup>();

builder.Services.AddScoped<IImageStorageService, ImageStorageService>();

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

app.UseAntiforgery();

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .HasApiVersion(new ApiVersion(2))
    .ReportApiVersions()
    .Build();

RouteGroupBuilder versionedGroup = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

app.MapEndpoints(versionedGroup);

if (app.Environment.IsDevelopment())
{
    var descriptors = app.DescribeApiVersions();

    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.Title = "Azure Shopping Cart";
        options.Theme = ScalarTheme.Default;
        options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
        options.CustomCss = "";
        options.ShowSidebar = true;
        options.DisableAgent();
        options.AddDocuments(descriptors.Select((d, index) => new ScalarDocument(
            Name: d.GroupName,
            Title: d.GroupName,
            IsDefault: index == descriptors.Count - 1)));
    });

    await app.ApplyMigrationsAsync();
    await app.SeedInitialDataAsync();
}

app.UseHttpsRedirection();

app.Run();