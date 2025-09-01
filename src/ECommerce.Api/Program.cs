using ECommerce.Api.Middlewares;
using ECommerce.Application;
using ECommerce.Application.Abstractions;
using ECommerce.Infrastructure.Balance;
using ECommerce.Infrastructure.Balance.Mapping;
using ECommerce.Infrastructure.Persistence;
using ECommerce.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;
using System.Net;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(ECommerce.Application.AssemblyMarker).Assembly);
});

builder.Services.AddValidatorsFromAssembly(typeof(ECommerce.Application.AssemblyMarker).Assembly);


builder.Services.AddAutoMapper(
    cfg => { },                                     
    typeof(AssemblyMarker).Assembly,               
    typeof(BalanceMappingProfile).Assembly         
);

//repository
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

//polly
IAsyncPolicy<HttpResponseMessage> retryPolicy =
    HttpPolicyExtensions.HandleTransientHttpError()
        .WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(200 * Math.Pow(2, i)));

IAsyncPolicy<HttpResponseMessage> circuitPolicy =
    HttpPolicyExtensions.HandleTransientHttpError()
        .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));

var retryHandler = new PolicyHttpMessageHandler(retryPolicy);
var circuitHandler = new PolicyHttpMessageHandler(circuitPolicy);

//typed httpclient
builder.Services.AddHttpClient<IBalanceClient, BalanceClient>(client =>
{
    var baseUrl = builder.Configuration["Balance:BaseUrl"]
                  ?? "https://balance-management-pi44.onrender.com/api/";
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
})
.AddPolicyHandler(retryPolicy)
.AddPolicyHandler(circuitPolicy);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseGlobalExceptions();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (db.Database.GetPendingMigrations().Any())
        db.Database.Migrate();
    else
        db.Database.EnsureCreated();
}

app.MapControllers();
app.Run();
