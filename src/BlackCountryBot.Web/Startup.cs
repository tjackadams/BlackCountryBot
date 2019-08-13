using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BlackCountryBot.Core.Infrastructure;
using BlackCountryBot.Core.Models.Phrases;
using BlackCountryBot.Web.Features.Phrases;
using BlackCountryBot.Web.Hubs;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BlackCountryBot.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<BlackCountryDbContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionString"]);
            });

            services.AddAutoMapper(typeof(Startup));

            services.AddMediatR(typeof(Startup).Assembly);
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CreateValidationBehavior<,>));

            services.AddSignalR();

            services
                .AddLogging(logging => logging.AddConsole())
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddControllersAsServices()
                .AddFluentValidation(o => o.RegisterValidatorsFromAssemblyContaining<Startup>());

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.AddScoped(typeof(IDbContextProvider<>), typeof(DbContextProvider<>));
            services.AddScoped<IRepository<Phrase>, Repository<BlackCountryDbContext, Phrase>>(
                sp => new Repository<BlackCountryDbContext, Phrase>(sp.GetRequiredService<IDbContextProvider<BlackCountryDbContext>>()));


            return new Container(rules =>
                    // optional: Enables property injection for Controllers
                    // In current setup `WithMef` it will be overriden by properties marked with `ImportAttribute`
                    rules.With(propertiesAndFields: request => request.ServiceType.Name.EndsWith("Controller")
                        ? PropertiesAndFields.Properties()(request)
                        : null)
                )
                .WithDependencyInjectionAdapter(services)
                .WithCompositionRoot<CompositionRoot>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSignalR(routes =>
            {
                routes.MapHub<PhrasesHub>("/hub/phrases");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }

    public class CompositionRoot
    {
        public CompositionRoot(IRegistrator r)
        {
            r.RegisterDelegate<ServiceFactory>(x => x.Resolve);
        }
    }

    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public ILogger<TRequest> Logger { get; private set; }
        public LoggingBehaviour(ILogger<TRequest> logger)
        {
            Logger = logger ?? NullLogger<TRequest>.Instance;
        }
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            using (Logger.BeginScope(request))
            {
                Logger.LogInformation("Calling handler...");
                TResponse response = await next();
                Logger.LogInformation("Called handler with result {0}", response);
                return response;
            }
        }
    }
}
