using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Features.Covers.Models;

public class CoverEntity
{
    [BsonId] public string Id { get; set; } = null!;

    [BsonElement("startDate")]
    public DateOnly StartDate { get; set; }

    [BsonElement("endDate")]
    public DateOnly EndDate { get; set; }

    [BsonElement("claimType")] public CoverType Type { get; set; }

    [BsonElement("premium")] public decimal Premium { get; set; }
}

public enum CoverType
{
    Yacht = 0,
    PassengerShip = 1,
    ContainerShip = 2,
    BulkCarrier = 3,
    Tanker = 4
}