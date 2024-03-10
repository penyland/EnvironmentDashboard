using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace Penyland.MicrosoftGraphClient;

public interface IGraphClient
{
    Task<Application?> GetApplicationAsync(string appId);

    Task<IReadOnlyCollection<Application>> GetApplicationsAsync(string? tag = default);

    Task<ServicePrincipal?> GetServicePrincipalAsync(string appId);

    Task<IReadOnlyCollection<ServicePrincipal>> GetServicePrincipalsAsync();

    Task<AppRoleAssignment?> AssignAppRoleToServicePrincipalAsync(string servicePrincipalId, string appRoleId, string resourceId);

    Task<IReadOnlyCollection<AppRoleAssignment>> ListAppRoleAssignmentsAsync(string servicePrincipalId);

    Task RemoveAppRoleAssignmentAsync(string servicePrincipalId, string appRoleAssignmentId);
}

public class GraphClient : IGraphClient
{
    private readonly GraphServiceClient graphServiceClient;

    public static GraphServiceClient CreateGraphServiceClient(AzureAdOptions options)
    {
        var clientSecretCredential = new ClientSecretCredential(options.TenantId, options.ClientId, options.ClientSecret);
        var graphClientService = new GraphServiceClient(
            clientSecretCredential,
            [
                "https://graph.microsoft.com/.default"
            ]);

        return graphClientService;
    }

    public GraphClient(GraphServiceClient graphServiceClient)
    {
        this.graphServiceClient = graphServiceClient;
    }

    public async Task<Application?> GetApplicationAsync(string appId)
    {
        var response = await graphServiceClient.Applications.GetAsync(requestConfig =>
        {
            requestConfig.QueryParameters.Filter = $"appId eq '{appId}'";
        });

        if (response != null && response.Value != null)
        {
            var responseValue = response.Value.First();

            return new Application
            {
                Id = responseValue.Id ?? string.Empty,
                AppId = responseValue.AppId ?? string.Empty,
                AppRoles = responseValue.AppRoles?.Select(t => t.DisplayName).ToList() ?? [],
                DisplayName = responseValue.DisplayName ?? string.Empty,
                IdentifierUris = responseValue.IdentifierUris?.ToList() ?? [],
                Tags = responseValue.Tags?.ToList() ?? []
            };
        }

        return default;
    }

    public async Task<IReadOnlyCollection<Application>> GetApplicationsAsync(string? tag = default)
    {
        var response = await graphServiceClient.Applications.GetAsync(config =>
        {
            if (string.IsNullOrEmpty(tag))
            {
                return;
            }
            else
            {
                config.QueryParameters.Filter = $"tags/any(t: t eq '{tag}')";
            }

            //config.QueryParameters.Select = ["id, appId, displayName"];
            config.QueryParameters.Orderby = ["displayName"];
            config.Headers.Add("ConsistencyLevel", "eventual");
        });

        if (response != null && response.Value != null)
        {
            var applications = new List<Application>();
            var l = response.Value.Select(app => new Application
            {
                Id = app.Id ?? string.Empty,
            });

            foreach (var application in response.Value)
            {
                applications.Add(new Application
                {
                    Id = application.Id ?? string.Empty,
                    AppId = application.AppId ?? string.Empty,
                    AppRoles = application.AppRoles?.Select(t => t.DisplayName).ToList() ?? [],
                    DisplayName = application.DisplayName ?? string.Empty,
                    IdentifierUris = application.IdentifierUris?.ToList() ?? [],
                    Tags = application.Tags?.ToList() ?? []
                });
            }

            return applications;
        }

        return [];
    }

    public async Task<ServicePrincipal?> GetServicePrincipalAsync(string appId)
    {
        var response = await graphServiceClient.ServicePrincipals.GetAsync(request =>
        {
            request.QueryParameters.Search = $"\"appId:{appId}\"";
            request.Headers.Add("ConsistencyLevel", "eventual");
        });

        if (response != null && response.Value != null)
        {
            var servicePrincipal = response.Value.First();
            if (servicePrincipal != null)
            {
                return new ServicePrincipal
                {
                    AppId = servicePrincipal.AppId ?? string.Empty,
                    DisplayName = servicePrincipal.DisplayName ?? string.Empty,
                    ObjectId = servicePrincipal.Id ?? string.Empty,
                    Tags = servicePrincipal.Tags?.ToList() ?? []
                };
            }
        }

        return null;
    }

    public async Task<IReadOnlyCollection<ServicePrincipal>> GetServicePrincipalsAsync()
    {
        var response = await graphServiceClient.ServicePrincipals.GetAsync(config =>
        {
            config.QueryParameters.Filter = "servicePrincipalType eq 'ManagedIdentity'";
            config.Headers.Add("ConsistencyLevel", "eventual");
        });

        if (response != null && response.Value != null)
        {
            var servicePrincipals = new List<ServicePrincipal>();
            foreach (var servicePrincipal in response.Value)
            {
                servicePrincipals.Add(new ServicePrincipal
                {
                    AppId = servicePrincipal.AppId ?? string.Empty,
                    DisplayName = servicePrincipal.DisplayName ?? string.Empty,
                    ObjectId = servicePrincipal.Id ?? string.Empty,
                    Tags = servicePrincipal.Tags?.ToList() ?? []
                });
            }

            return servicePrincipals;
        }

        return [];
    }

    public async Task<AppRoleAssignment?> AssignAppRoleToServicePrincipalAsync(string servicePrincipalId, string appRoleId, string resourceId)
    {
        var appRoleAssignment = new AppRoleAssignment
        {
            PrincipalId = Guid.Parse(servicePrincipalId),
            ResourceId = Guid.Parse(resourceId),
            AppRoleId = Guid.Parse(appRoleId)
        };

        var result = await graphServiceClient.ServicePrincipals[servicePrincipalId].AppRoleAssignments.PostAsync(appRoleAssignment);

        return result;
    }

    public async Task RemoveAppRoleAssignmentAsync(string servicePrincipalId, string appRoleAssignmentId)
    {
        await graphServiceClient.ServicePrincipals[servicePrincipalId].AppRoleAssignedTo[appRoleAssignmentId].DeleteAsync();
    }

    public async Task<IReadOnlyCollection<AppRoleAssignment>> ListAppRoleAssignmentsAsync(string servicePrincipalId)
    {
        var response = await graphServiceClient.ServicePrincipals[servicePrincipalId].AppRoleAssignments.GetAsync();

        if (response != null && response.Value != null)
        {
            var appRoleAssignments = new List<AppRoleAssignment>();
            foreach (var appRoleAssignment in response.Value)
            {
                appRoleAssignments.Add(new AppRoleAssignment
                {
                    PrincipalId = appRoleAssignment.PrincipalId,
                    ResourceId = appRoleAssignment.ResourceId,
                    AppRoleId = appRoleAssignment.AppRoleId
                });
            }

            return appRoleAssignments;
        }

        return [];
    }
}
