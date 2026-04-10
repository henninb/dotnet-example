using human_module;
using Microsoft.AspNetCore.Mvc;

// Development: store HumanConfiguration secrets with `dotnet user-secrets` (not in repo). Production: env vars like HumanConfiguration__px_app_id.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});
builder.Services.Configure<EnforcerConfig>(builder.Configuration.GetSection("HumanConfiguration"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseHumanMiddleware();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
