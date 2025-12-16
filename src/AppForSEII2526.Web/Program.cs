using AppForSEII2526.Web;
using AppForSEII2526.Web.API; // Para registrar el cliente HTTP
using AppForSEII2526.Web.Components;
using AppForSEII2526.Web.Components.Account;
using AppForSEII2526.Web.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();


// IMPORTANTE: El puerto 7067 sale de tu launchSettings.json de la API (https)

//MODIFICACIONES: debido a que el mero de presionar la pestaña donde tenemos la selección de compras provocaba la combustión instantánea de mi pantalla, he decidido usar el pratón Factory:

//Añadimos un cliente estandar (AddHttpClient), del tipo generado por Swagger. Tras mirar el constructor (http, sp) dentro de la definición,
// me doy cuenta que el sistema no sabe que poner dentro de sp, por lo que tenemos que darle la url manualmente.

builder.Services.AddHttpClient<AppForSEII2526APIClient>()
    .AddTypedClient<AppForSEII2526APIClient>((http, sp) =>
    {
        var apiUrl = "https://localhost:7067/";
        return new AppForSEII2526APIClient(apiUrl, http);
    });

// Registramos el StateContainer para gestionar el estado de la compra (carrito)
builder.Services.AddScoped<PurchaseStateContainer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
