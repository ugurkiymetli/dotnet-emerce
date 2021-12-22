using AutoMapper;
using Emerce_API.Infrastructure;
using Emerce_API.Services;
using Emerce_Service.Category;
using Emerce_Service.Product;
using Emerce_Service.User;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Emerce_API
{
    public class Startup
    {
        public Startup( IConfiguration configuration )
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        readonly string AllowAllHeaders = "_allowAllHeaders";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices( IServiceCollection services )
        {
            var _mappingProfile = new MapperConfiguration(mp => { mp.AddProfile(new MappingProfile()); });
            //mapper configuration
            IMapper mapper = _mappingProfile.CreateMapper();
            services.AddSingleton(mapper);

            //services for user, product, category
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<ICategoryService, CategoryService>();

            //inMemory cache
            //services.AddMemoryCache();

            //redis - distributed memory cache
            services.AddStackExchangeRedisCache(action =>
            {
                action.Configuration = "localhost:6379";
            });

            // inject our auth filters 
            services.AddScoped<LoginFilter>();
            services.AddScoped<AdminFilter>();
            //CORS - to use from outside api calls
            services.AddCors(options =>
            {
                options.AddPolicy(name: AllowAllHeaders,
                                  builder =>
                                  {
                                      builder/*.WithOrigins("http://localhost:3001/")*/
                                      .AllowAnyHeader()
                                      .AllowAnyMethod()
                                      .AllowAnyOrigin();
                                  });
            });

            services.AddControllers();

            //swagger config
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Emerce_API", Version = "v1" });
            });
            //inject jobservice interface
            services.AddScoped<IJobService, JobService>();

            //hangfire - configure sql connection - HangfireDBConnection is in Emerce-API/appsettings.json 
            services.AddHangfire(x =>
            {
                x.UseSqlServerStorage(Configuration.GetConnectionString("HangfireDBConnection"));
            });
            services.AddHangfireServer();

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
        {
            if ( env.IsDevelopment() )
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Emerce_API v1"));
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(AllowAllHeaders);
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            //enable hangfire dashboard 
            app.UseHangfireDashboard();
            //adding reccurring jobs at startup
            RecurringJob.AddOrUpdate("UpdatePricesUSD", () => new JobService().UpdatePrices(), Cron.Hourly);
            RecurringJob.AddOrUpdate("CleanUserTable", () => new JobService().CleanUserTable(), Cron.Daily);
            RecurringJob.AddOrUpdate("SendWelcomeMail", () => new JobService().SendWelcomeMail(), Cron.Daily);
        }
    }
}