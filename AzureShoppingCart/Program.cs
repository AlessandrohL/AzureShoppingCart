using Asp.Versioning;
using Asp.Versioning.Builder;
using AzureShoppingCart.Data;
using AzureShoppingCart.Data.Interceptors;
using AzureShoppingCart.Extensions;
using AzureShoppingCart.Identity;
using AzureShoppingCart.Identity.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<IUserContext, UserContext>();

builder.Services.ConfigureOptions<SeedAdminOptionsSetup>();

builder.Services.AddDbContext<ApplicationDbContext>((sp, options) => options
    .UseSqlServer(builder.Configuration.GetConnectionString("Database"))
    .AddInterceptors(sp.GetRequiredService<AuditableInterceptor>()));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddProblemDetails();

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

var app = builder.Build();

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