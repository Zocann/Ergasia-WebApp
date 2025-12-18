using Ergasia_WebApp.DTOs.Rating;
using Ergasia_WebApp.Middleware;
using Ergasia_WebApp.Services;
using Ergasia_WebApp.Services.Interfaces;
using Ergasia_WebApp.Services.Model;
using Ergasia_WebApp.Services.Model.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(policy =>
    policy.AddDefaultPolicy(p => p.WithOrigins("*")));

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddHttpClient(
    "API", (client) => 
        client.BaseAddress = new Uri(builder.Configuration.GetSection("API").GetValue<string>("BaseAddress") 
                                     ?? throw new InvalidOperationException("No API base address found")));

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<IEmployerService, EmployerApiService>();
builder.Services.AddScoped<IWorkerService, WorkerApiService>();
builder.Services.AddScoped<IJobService, JobApiService>();
builder.Services.AddScoped<IWorkerJobService, WorkerJobApiService>();
builder.Services.AddScoped<IJobRatingService, JobRatingApiService>();
builder.Services.AddScoped<IUserService, UserApiService>();
builder.Services.AddScoped<IEmployerRatingService, EmployerRatingApiService>();
builder.Services.AddScoped<IWorkerRatingService, WorkerRatingApiService>();

var app = builder.Build();

app.UseCors(policy => policy
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin()
);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
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