namespace DuitTracker.Api.Shared.Domain;

public class Transaction : BaseClass
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public decimal Amount { get; set; }
    public string Note { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string ReferenceNo { get; set; } = string.Empty;
    public string AttachmentUrl { get; set; } = string.Empty;
    public Category Category { get; set; } = null!;
    public PaymentMethod PaymentMethod { get; set; } = null!;
}