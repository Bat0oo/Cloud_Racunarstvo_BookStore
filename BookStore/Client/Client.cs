using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;

namespace Client
{
    internal sealed class Client : StatelessService
    {
        public Client(StatelessServiceContext context)
            : base(context)
        { }

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
                        builder.Services.AddControllersWithViews();

                        var app = builder.Build();
                        
                        // Configure the HTTP request pipeline.
                        if (!app.Environment.IsDevelopment())
                        {
                        app.UseExceptionHandler("/Home/Error");
                        }
                        app.UseStaticFiles();

                        app.UseRouting();

                        app.UseAuthorization();

                        app.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");


                        return app;
                        //builder.Services.AddControllersWithViews();
                        //builder.Services.AddMvc(option => option.EnableEndpointRouting = false);
                        //// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                        //builder.Services.AddEndpointsApiExplorer();
                        //builder.Services.AddSwaggerGen();

                        //var app = builder.Build();
                        
                        //// Configure the HTTP request pipeline.
                        //if (app.Environment.IsDevelopment())
                        //{
                        //    //app.UseSwagger();
                        //    //app.UseSwaggerUI();
                        //}

                        //app.UseRouting();
                        //app.UseMvcWithDefaultRoute();
                        //app.UseAuthorization();

                        //app.MapControllers();
                        //app.Run();

                        //return app;
                    }))
            };
        }
    }
}