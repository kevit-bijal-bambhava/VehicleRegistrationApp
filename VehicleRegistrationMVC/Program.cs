using Serilog;
using VehicleRegistrationMVC.Filters.ActionFilters;
using VehicleRegistrationMVC.Services;

var builder = WebApplication.CreateBuilder(args);

//Configure Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {

    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
    .ReadFrom.Services(services); //read out current app's services and make them available to serilog
});

builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<VehicleService>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ValidateJwtTokenFilter>();
builder.Services.AddScoped<ModelStateValidationFilter>();
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:5239/");
    });
});

var app = builder.Build();

app.UseSerilogRequestLogging();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
