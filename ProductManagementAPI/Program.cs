using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using ProductManagementAPI.Data;
using ProductManagementAPI.Repositories;
using ProductManagementAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Text.Json;
using ProductManagementAPI.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register the ApplicationDbContext with the connection string from appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the HttpClient factory which allows me to create HttpClient instances
builder.Services.AddHttpClient();

// Register CORS(Cross-origin resource sharing) services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< RETRY POLICY CODE >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

// Configure a retry policy that will handle transient HTTP errors by waiting and retrying
// a specified number of times with a delay between retries.
var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError() // Handles common transient errors.
    .WaitAndRetryAsync(new[]
    {
        TimeSpan.FromSeconds(1), // Wait 1 second before the first retry.
        TimeSpan.FromSeconds(5), // Wait 5 seconds before the second retry.
        TimeSpan.FromSeconds(10) // Wait 10 seconds before the third retry.
    });

// Register the ExternalProductService for dependency injection and associate it with the retry policy.
builder.Services.AddHttpClient<ExternalProductService>()
    .AddPolicyHandler(retryPolicy);


builder.Services.AddHostedService<DailyUpdateService>(); // Registering my DailyUpdateService (responsible for making daily API calls to AMAZON)
builder.Services.AddSingleton<ExternalProductService>(); 


// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< DEPENDENCY INJECTION REGISTRATION >>>>>>>>>>>>>>>>>>>>>>>>>>>>>
builder.Services.AddScoped<IProductRepository, ProductRepository>(); //Registering IProductRepository for dependency injection

// Add Swagger generation tool.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin"); // Use the CORS policy

app.UseAuthorization();

app.MapControllers();

// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Db Seeding Before the Application Runs >>>>>>>>>>>>>>>>>>>>>>>>>>>>>
var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
    // Link to the json string of my seed data (seed products)
    var jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "seedData.json");
    var jsonString = File.ReadAllText(jsonFilePath);
    var products = JsonConvert.DeserializeObject<List<Product>>(jsonString);
    DataSeeder.SeedData(services, products); // Make sure to pass the list of products
}
finally
{
    scope.Dispose();
}

//Run the application 
app.Run();
