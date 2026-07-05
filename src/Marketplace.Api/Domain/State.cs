namespace Marketplace.Api.Domain;

public sealed class State
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
}
