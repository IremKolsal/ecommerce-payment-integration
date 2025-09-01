using System.Text.Json.Serialization;

namespace ECommerce.Infrastructure.Balance.Models;

internal sealed record ResponseEnvelope<T>(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("data")] T? Data
);
