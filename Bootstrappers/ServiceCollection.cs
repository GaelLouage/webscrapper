using WebScrapper.Services.Classes;
using WebScrapper.Services.Interfaces;

namespace WebScrapper.Bootstrappers
{
    public static class ServiceCollection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services.AddEndpointsApiExplorer()
                           .AddSwaggerGen()
                           .AddScoped<IWebScrapperService, WebScrapperService>();
        }
    }
}
