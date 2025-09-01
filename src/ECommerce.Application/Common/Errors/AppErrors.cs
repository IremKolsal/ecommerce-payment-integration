namespace ECommerce.Application.Common.Errors;

public static class AppErrors
{
    public static class OrderErrors
    {
        public static Exception NotFound(string externalOrderId) =>
            new KeyNotFoundException($"Order '{externalOrderId}' not found.");

        public static Exception MustBeBlocked(string externalOrderId, string currentState) =>
            new InvalidOperationException($"Order '{externalOrderId}' state must be 'blocked' but was '{currentState}'.");
    }
}
