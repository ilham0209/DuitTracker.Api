namespace DuitTracker.Api.Shared.Domain;

public class PaymentMethod : BaseClass
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}