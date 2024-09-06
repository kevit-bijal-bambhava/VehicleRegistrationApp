using Quartz;
using Serilog;
using System.Configuration;
using VehicleRegistrationMVC.Filters.ActionFilters;
using VehicleRegistrationMVC.Services;

var builder = WebApplication.CreateBuilder(args);

# region Add Quartz services to the DI container
builder.Services.AddQuartz(q =>
{
    // Create a job
    q.UseMicrosoftDependencyInjectionJobFactory();

    var jobKey = new JobKey("LogMessageJob");
    q.AddJob<LogMessageJob>(opts => opts.WithIdentity(jobKey));

    string? expr = builder.Configuration["Quartz:LogMessageJob"];

    // Create a trigger to run every 5 seconds
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("LogMessageJob-trigger")
        .WithCronSchedule(expr)
    );
});
#endregion

// Add Quartz hosted service
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

//Configure Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {

    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
    .ReadFrom.Services(services); //read out current app's services and make them available to serilog
});

builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddTransient<AccountService>();
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
Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
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
