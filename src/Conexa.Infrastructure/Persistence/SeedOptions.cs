namespace Conexa.Infrastructure.Persistence;

public class SeedOptions
{
    public const string SectionName = "Seed";

    public bool Enabled { get; set; }
    public string? AdminEmail { get; set; }
    public string? AdminPassword { get; set; }
    public string? RegularEmail { get; set; }
    public string? RegularPassword { get; set; }
}
