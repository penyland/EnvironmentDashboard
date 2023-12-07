namespace EnvironmentDashboard.Features.Environments;

public record Environment
{
    public Environment(long id, string name, string? url = null, string? description = null)
    {
        Id = id;
        Name = name;
        Url = url;
        Description = description;
    }

    public long Id { get; init; }

    public string Name { get; init; }

    public string? Url { get; init; }

    public string? Description { get; init; }

    public List<Application> Applications { get; init; } = new();
}

public record Application(string Name, string Version, string OS);