namespace DuitTracker.Api.Shared.Domain;

public class Transaction : BaseClass
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}