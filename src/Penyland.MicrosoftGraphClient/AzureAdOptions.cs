namespace Penyland.MicrosoftGraphClient;

public class AzureAdOptions
{
    public string? ClientId { get; set; }

    public string? TenantId { get; set; }

    public string? ClientSecret { get; set; }

    public string? Authority { get; set; }

    public string? GraphResourceId { get; set; }

    public string? GraphResourceIdSecret { get; }
}
