namespace Penyland.MicrosoftGraphClient;

public record ServicePrincipal
{
    public string Id { get; init; }

    public string AppId { get; init; }

    public string DisplayName { get; init; }

    public string ObjectId { get; init; }

    public List<string> Tags { get; init; } = [];
}
