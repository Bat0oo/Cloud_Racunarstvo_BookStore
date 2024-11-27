using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;

namespace Client.UI
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class UI : StatelessService
    {
        public UI(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        var builder = WebApplication.CreateBuilder();

                        builder.Services.AddSingleton<StatelessServiceContext>(serviceContext);
                        builder.WebHost
                                    .UseKestrel()
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url);
                        
                        // Add services to the container.
                        builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
                        //builder.Services.AddControllersWithViews();
                        builder.Services.AddMvc(option => option.EnableEndpointRouting = false);
                        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                        //builder.Services.AddEndpointsApiExplorer();
                        //builder.Services.AddSwaggerGen();

                        var app = builder.Build();
                        
                        // Configure the HTTP request pipeline.
                        if (app.Environment.IsDevelopment())
                        {
                            app.UseDeveloperExceptionPage();
                        }
                        else
                        {
                            app.UseExceptionHandler("/Home/Error");
                            app.UseHsts();
                        }


                        app.UseStaticFiles();
                        app.UseRouting();
                        app.UseMvcWithDefaultRoute();
                        app.UseAuthorization();

                        app.MapControllers();
                        app.Run();

                        return app;
                    }))
            };
        }
    }
}
