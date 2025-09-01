using ECommerce.Infrastructure.Balance.Models;
using ECommerce.Infrastructure.Common.Errors;

namespace ECommerce.Infrastructure.Balance;

internal static class ResponseGuard
{
    public static void EnsureOk<T>(ResponseEnvelope<T> env, string endpoint)
    {
        if (!env.Success) throw new UpstreamServiceException(endpoint, env.Message);
    }

    public static T ThrowIfNull<T>(T? value, string where) where T : class
        => value ?? throw new PayloadMissingException(where);
}