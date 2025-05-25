using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Amazon.DynamoDBv2;
using Amazon;
using Amazon.DynamoDBv2.DataModel;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

var builder = WebApplication.CreateBuilder(args);

// Configure AWS DynamoDB
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
builder.Services.AddSingleton(new Cloudinary(new Account(
    builder.Configuration["Cloudinary:CloudName"],
    builder.Configuration["Cloudinary:ApiKey"],
    builder.Configuration["Cloudinary:ApiSecret"]
)));
// Register Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Website API", Version = "v1" });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGitHubPages",
        builder => builder
            .WithOrigins("https://tam02112003.github.io", "http://localhost:3000", "https://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader());
});
builder.Configuration.AddEnvironmentVariables();
// Register services for Controllers
builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("AllowGitHubPages");

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add CORS middleware
app.UseCors("AllowAllOrigins");

app.UseAuthorization();

// Add Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Website API V1");
    c.RoutePrefix = "swagger";
});

app.MapControllers();

app.Run();