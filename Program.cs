using Microsoft.EntityFrameworkCore;
using warehouseapp.Middleware;
using warehouse.Data;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorBootstrap();

builder.Services.AddDbContext<WarehouseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("https://localhost:5001/");
});

builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// // Map Blazor components
app.MapRazorComponents<warehouseapp.Components.App>()
    .AddInteractiveServerRenderMode();


app.Run();
