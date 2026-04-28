using Asp.Versioning;
using Asp.Versioning.Builder;
using AzureShoppingCart;
using AzureShoppingCart.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Scalar.AspNetCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

var builder = WebApplication.CreateBuilder(args)
    .AddApiServices()
    .AddErrorHandling()
    .AddDatabase()
    .AddAzureServices()
    .AddApplicationServices()
    .AddAuthenticationServices();

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