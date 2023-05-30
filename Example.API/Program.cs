using Elastic.Apm.AspNetCore;
using Example.API.Extensions;
using Example.API.Middlewares;
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddPersistense();
builder.AddApplicationModule();
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(Assembly.Load("Example.Application"));
builder.Services.AddAutoMapper(Assembly.Load("Example.Application"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.AddSwaggerConfiguration();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddControllersWithViews();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

app.UseSwaggerSetup();
app.UseApiVersioning();
app.UseElasticApm();
app.UseContextMigrations();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<ErrorMiddleware>();

app.UseCors(builder =>
{
    builder.AllowAnyOrigin();
    builder.AllowAnyMethod();
    builder.AllowAnyHeader();
});


app.Run();

namespace Example.API.Startup
{
    public partial class Program
    {
        protected Program() { }
    }
}