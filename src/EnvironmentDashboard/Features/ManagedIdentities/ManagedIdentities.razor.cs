using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Penyland.MicrosoftGraphClient;
using System.Diagnostics;

namespace EnvironmentDashboard.Features.ManagedIdentities;

public partial class ManagedIdentities
{
    private readonly GridSort<Identity> sortByName = GridSort<Identity>
        .ByAscending(p => p.DisplayName)
        .ThenAscending(p => p.AppId);

    private readonly GridSort<Identity> sortByAppId = GridSort<Identity>
        .ByAscending(p => p.AppId)
        .ThenAscending(p => p.DisplayName);

    private readonly GridSort<Identity> sortById = GridSort<Identity>
        .ByAscending(p => p.Id)
        .ThenAscending(p => p.DisplayName);

    public bool IsBusy { get; set; }

    public IQueryable<Identity> Identities { get; set; } = new List<Identity>().AsQueryable();

    public Identity SelectedIdentity { get; set; }

    [Inject]
    public IGraphClient GraphClient { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        //Identitys = new List<Identity>
        //{
        //    new()
        //    {
        //        AppId = "2132d464-fd92-4d1c-a8d9-48a47bec2994",
        //        Id = "1601866c-a404-4df5-b9c2-4ac5d7b8a45b",
        //        DisplayName = "Identity 1",
        //        Tags = ["tag1", "tag2"]
        //    },
        //    new()
        //    {
        //        AppId = "53b07f7d-e1c3-436c-9859-7ffdb3ae88c8",
        //        Id = "7ca1c692-2ab9-483e-8c81-ce4a596e5458",
        //        DisplayName = "Identity 2"
        //    },
        //    new()
        //    {
        //        AppId = "f3e3e3e3-3e3e-3e3e-3e3e-3e3e3e3e3e3e",
        //        Id = "6fe4f9d2-a49b-46aa-bb2b-10e5bda4bf11",
        //        DisplayName = "Identity 3"
        //    }
        //}.AsQueryable();
    }

    internal void OnBreakpointEnterHandler(GridItemSize size)
    {
        Console.WriteLine($"Breakpoint entered: {size}");
        Debug.WriteLine($"Breakpoint entered: {size}");
    }

    internal Task OnAppIdClickAsync(string appId)
    {
        Debug.WriteLine($"AppId clicked: {appId}");

        var application = Identities.FirstOrDefault(a => a.AppId == appId);
        if (application is not null)
        {
            SelectedIdentity = application;
        }

        return Task.CompletedTask;
    }

    internal async Task OnAppRegistrationsClicked()
    {
        await GetApplicationsFromGraphApiAsync();
    }

    internal async Task OnManagedIdentitiesClicked()
    {
        IsBusy = true;
        var managedIdentities = await GraphClient.GetServicePrincipalsAsync();
        Identities = managedIdentities
            .OrderBy(t => t.DisplayName)
            .Select(t => new Identity
            {
                AppId = t.AppId,
                DisplayName = t.DisplayName,
                Id = t.ObjectId,
                Tags = t.Tags,
                IdentityType = IdentityType.ManagedIdentity
            })
            .AsQueryable();
        IsBusy = false;
    }

    internal async Task GetApplicationsFromGraphApiAsync()
    {
        IsBusy = true;
        var applications = await GraphClient.GetApplicationsAsync();

        foreach (var application in applications)
        {
            var servicePrincipal = await GraphClient.GetServicePrincipalAsync(application.AppId);
        }

        Identities = applications
            .OrderBy(t => t.DisplayName)
            .Select(t => new Identity
            {
                AppId = t.AppId,
                DisplayName = t.DisplayName,
                Id = t.Id,
                Tags = t.Tags,
                IdentityType = IdentityType.AppRegistration
            })
            .AsQueryable();

        IsBusy = false;
    }

    public enum IdentityType
    {
        AppRegistration,
        ManagedIdentity,
        ServicePrincipal
    }

    public record class Identity
    {
        public string Id { get; init; } = string.Empty;

        public string AppId { get; init; } = string.Empty;

        public string AppIdUri { get; init; } = string.Empty;

        public string DisplayName { get; init; } = string.Empty;

        public List<string> AppRoles { get; init; } = new();

        public List<string> IdentifierUris { get; init; } = new();

        public List<string> Tags { get; init; } = new();

        public IdentityType IdentityType { get; init; } = IdentityType.ManagedIdentity;
    }
}
