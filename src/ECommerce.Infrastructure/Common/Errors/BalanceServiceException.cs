namespace ECommerce.Infrastructure.Common.Errors;

public abstract class BalanceServiceException : InvalidOperationException
{
    protected BalanceServiceException(string message) : base(message) { }
}

public sealed class UpstreamServiceException : BalanceServiceException
{
    public string Endpoint { get; }
    public UpstreamServiceException(string endpoint, string? message)
        : base($"Upstream error ({endpoint}): {message}") => Endpoint = endpoint;
}

public sealed class EmptyResponseException : BalanceServiceException
{
    public string Endpoint { get; }
    public EmptyResponseException(string endpoint)
        : base($"Empty response ({endpoint}).") => Endpoint = endpoint;
}

public sealed class PayloadMissingException : BalanceServiceException
{
    public string Where { get; }
    public PayloadMissingException(string where)
        : base($"Payload missing ({where}).") => Where = where;
}

