using System.Text.Json.Serialization;

namespace ECommerce.Infrastructure.Balance.Models;

internal sealed record ApiEnvelope<T>(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("data")] T? Data
);
