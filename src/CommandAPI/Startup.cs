using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using AutoMapper;
using Newtonsoft.Json.Serialization;

namespace CommandAPI
{
    public class Startup
    {
        public IConfiguration Configuration {get;}
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Import secrets into Connection String
            var builder = new NpgsqlConnectionStringBuilder();

            // retrieve base connection string from appsettings.development.json
            builder.ConnectionString = Configuration.GetConnectionString("PostgreSqlConnection");
            
            // append string using values from secrets.json
            builder.Username = Configuration["UserID"];
            builder.Password = Configuration["Password"];

            // use constructed connection string using builder object
            services.AddDbContext<CommandContext>(opt => opt.UseNpgsql(builder.ConnectionString));
            
            // require NewtonSoftJson package within controller, to correctly
            // parse Patch document
            services.AddControllers().AddNewtonsoftJson( s => {
                s.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            services.AddControllers();
            
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<ICommandAPIRepo, SqlCommandAPIRepo>();
            // services.AddScoped<ICommandAPIRepo, MockCommandAPIRepo>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
               /*  
               endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                }); 
                */
                endpoints.MapControllers();
            });
        }
    }
}
