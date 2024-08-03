var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

string connectionString = @"server=DESKTOP-9VFSRAA; user id=sa; password=123; database=LiteCommerceDB;TrustServerCertificate=true";
SV21T1080027.BusinessLayers.Configuration.Initialize(connectionString);

app.Run();
