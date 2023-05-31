using Example.Application.Modules;
using Example.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Example.API.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtension
    {
        public static WebApplicationBuilder AddPersistense(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' not found.");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    builder => builder.MigrationsAssembly("Example.Data")));

            return builder;
        }

        public static WebApplicationBuilder AddApplicationModule(this WebApplicationBuilder builder)
        {
            ApplicationModule.Register(builder.Services);

            return builder;
        }

        public static WebApplicationBuilder AddSwaggerConfiguration(this WebApplicationBuilder builder)
        {

            builder.Services.AddApiVersioning(c =>
            {
                c.DefaultApiVersion = new ApiVersion(1, 0);
                c.ReportApiVersions = true;
                c.AssumeDefaultVersionWhenUnspecified = true;
            });

            builder.Services.AddVersionedApiExplorer(c =>
            {
                c.GroupNameFormat = "'v'VVV";
                c.SubstituteApiVersionInUrl = true;
            });


            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Example.Services.Api",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Ismael Alves",
                        Email = "cearaismael1997@gmail.com"
                    }
                });
                c.SchemaFilter<EnumSchemaFilter>();
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            return builder;
        }

        private sealed class EnumSchemaFilter : ISchemaFilter
        {
            public void Apply(OpenApiSchema schema, SchemaFilterContext context)
            {
                if (context.Type.IsEnum)
                {
                    schema.Type = "string";
                    schema.Format = "string";
                    schema.Enum.Clear();
                    Enum.GetNames(context.Type)
                        .ToList()
                        .ForEach(n => schema.Enum.Add(new OpenApiString(n)));
                }
            }
        }
    }
}