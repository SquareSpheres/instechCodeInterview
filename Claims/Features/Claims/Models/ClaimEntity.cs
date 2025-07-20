using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Features.Claims.Models;

public class ClaimEntity
{
    [BsonId] public string Id { get; set; } = null!;

    [BsonElement("coverId")] public string CoverId { get; set; } = null!;

    [BsonElement("created")] public DateOnly Created { get; set; }

    [BsonElement("name")] public string Name { get; set; } = null!;

    [BsonElement("claimType")] public ClaimType Type { get; set; }

    [BsonElement("damageCost")] public decimal DamageCost { get; set; }
}

public enum ClaimType
{
    Collision = 0,
    Grounding = 1,
    BadWeather = 2,
    Fire = 3
}