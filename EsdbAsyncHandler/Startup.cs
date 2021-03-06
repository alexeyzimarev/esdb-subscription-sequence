using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace EsdbAsyncHandler {
    public class Startup {
        public void ConfigureServices(IServiceCollection services) {
            services.AddHostedService<ProducerService>();
            services.AddHostedService<ConsumerService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            app.UseRouting();

            app.UseEndpoints(endpoints => endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); }));
        }
    }
}
