namespace Penyland.MicrosoftGraphClient;

public record Application
{
    public string Id { get; init; }

    public string AppId { get; init; }

    public string AppIdUri => IdentifierUris.FirstOrDefault() ?? string.Empty;

    public List<string?> AppRoles { get; init; } = [];

    public string DisplayName { get; init; }

    public IEnumerable<string> IdentifierUris { get; init; } = [];

    public List<string> Tags { get; init; } = [];
}
