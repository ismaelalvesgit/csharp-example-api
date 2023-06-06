using Elastic.Apm.AspNetCore;
using Example.BackgroundTasks.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddPersistense();
builder.AddApplicationModule();
builder.AddCronJobs();
builder.AddConsumers();
builder.AddLibs();

var app = builder.Build();

app.UseElasticApm();

app.Run();

namespace Example.BackgroundTasks.Startup
{
    public partial class Program
    {
        protected Program() { }
    }
}