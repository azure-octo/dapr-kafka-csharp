namespace Dapr.Examples.Pubsub.Consumer
{
    using Dapr.Client;
    using Dapr.Examples.Pubsub.Models;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDaprClient();
            services.AddSingleton(serializerOptions);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCloudEvents();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapPost("sampletopic", this.SubscribeMessage).WithTopic("sampletopic");
            });
        }

        // Receive a "Post" object from the previous app in the pipeline.
        private async Task SubscribeMessage(HttpContext context)
        {
            Console.WriteLine($"Message is delivered.");

            var client = context.RequestServices.GetRequiredService<DaprClient>();
            var message = await JsonSerializer.DeserializeAsync<SocialMediaMessage>(context.Request.Body, serializerOptions);

            Console.WriteLine($"message id: {message.MessageId}");
            Console.WriteLine($"message context: {message.Message}");
            Console.WriteLine($"message creation time: {message.PreviousAppTimestamp}");
        }
    }
}
