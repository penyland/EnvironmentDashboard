namespace EnvironmentDashboard.Features.ManagedIdentities;

public partial class ManagedIdentities
{
    public bool IsBusy { get; set; }

    public IQueryable<Application> Applications { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Applications = new List<Application>
        {
            new()
            {
                AppId = "2132d464-fd92-4d1c-a8d9-48a47bec2994",
                Id = "1601866c-a404-4df5-b9c2-4ac5d7b8a45b",
                DisplayName = "Application 1"
            },
            new()
            {
                AppId = "53b07f7d-e1c3-436c-9859-7ffdb3ae88c8",
                Id = "7ca1c692-2ab9-483e-8c81-ce4a596e5458",
                DisplayName = "Application 2"
            },
            new()
            {
                AppId = "f3e3e3e3-3e3e-3e3e-3e3e-3e3e3e3e3e3e",
                Id = "6fe4f9d2-a49b-46aa-bb2b-10e5bda4bf11",
                DisplayName = "Application 3"
            }
        }.AsQueryable();
    }
}
