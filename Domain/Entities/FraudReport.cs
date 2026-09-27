namespace FraudexPy.Api.Domain.Entities;

public class FraudReport
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SuspectIdentity { get; set; } = string.Empty;
    public decimal EstimatedLoss { get; set; }
    public string Currency { get; set; } = "USD";
    public string Status { get; set; } = "Pending"; // "Pending", "UnderInvestigation", "ConfirmedFraud", "Dismissed"
    public string RiskLevel { get; set; } = "Medium"; // "Low", "Medium", "High", "Critical"
    public string? AttachmentObjectName { get; set; }
    public string? AttachmentFileName { get; set; }
    public string? AttachmentContentType { get; set; }
    public DateTime IncidentDate { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Guid ReportedByUserId { get; set; }
    public User? ReportedByUser { get; set; }
}
