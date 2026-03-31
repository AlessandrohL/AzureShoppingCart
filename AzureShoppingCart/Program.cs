using AzureShoppingCart.Data;
using AzureShoppingCart.Data.Interceptors;
using AzureShoppingCart.Extensions;
using AzureShoppingCart.Identity;
using AzureShoppingCart.Identity.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Azure Shopping Cart";
        options.Theme = ScalarTheme.Default;
        options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
        options.CustomCss = "";
        options.ShowSidebar = true;
        options.DisableAgent();
    });

    await app.ApplyMigrationsAsync();
    await app.SeedInitialDataAsync();
}

app.UseHttpsRedirection();

app.Run();