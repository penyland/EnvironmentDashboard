namespace EnvironmentDashboard.Features.ManagedIdentities;

public class Application
{
    public string AppId { get; set; }

    public string Id { get; set; }

    public string DisplayName { get; set; }

    public List<string> Tags { get; set; } = [];
}
