using Ergasia_WebApp.ApiRepositories;
using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.Middleware;
using Ergasia_WebApp.Services;
using Ergasia_WebApp.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddHttpClient(
    "API", (HttpClient client) => 
        client.BaseAddress = new Uri(builder.Configuration.GetSection("API").GetValue<string>("BaseAddress") 
                                     ?? throw new InvalidOperationException("No API base address found")));

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<IEmployerApiRepository, EmployerApiRepository>();
builder.Services.AddScoped<IWorkerApiRepository, WorkerApiRepository>();
builder.Services.AddScoped<IJobApiRepository, JobApiRepository>();
builder.Services.AddScoped<IUserApiRepository, UserApiRepository>();
builder.Services.AddScoped<IRatingApiRepository, RatingApiRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseTokenRefresh();

app.MapStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();

app.Run();