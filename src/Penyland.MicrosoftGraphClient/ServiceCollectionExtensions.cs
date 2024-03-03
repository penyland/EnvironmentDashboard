using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Penyland.MicrosoftGraphClient;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGraphClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IGraphClient>(provider =>
        {
            var options = configuration.GetRequiredSection("AzureAd").Get<AzureAdOptions>();
            ArgumentNullException.ThrowIfNull(options, nameof(options));
            return new GraphClient(GraphClient.CreateGraphServiceClient(options));
        });

        return services;
    }
}
