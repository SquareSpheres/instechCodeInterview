namespace Claims.Auditing;

public abstract class BaseAuditEntity
{
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public string? HttpRequestType { get; set; }
}