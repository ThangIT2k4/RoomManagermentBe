using System.Reflection;
using RoomManagerment.Shared.Common;

namespace RoomManagerment.Shared.MediatR;

internal static class ResultPipeline
{
    private static readonly Type ResultOpenGeneric = typeof(Result<>);

    public static bool IsResultResponse(Type responseType) =>
        responseType == typeof(Result) ||
        responseType.IsGenericType && responseType.GetGenericTypeDefinition() == ResultOpenGeneric;

    public static object FailureInstance(Type responseType, Error error)
    {
        if (responseType == typeof(Result))
        {
            return Result.Failure(error);
        }

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == ResultOpenGeneric)
        {
            var method = responseType.GetMethod("Failure", BindingFlags.Public | BindingFlags.Static, [typeof(Error)]);
            if (method is null)
            {
                throw new InvalidOperationException($"Type {responseType} has no static Failure(Error).");
            }

            return method.Invoke(null, [error])!;
        }

        throw new InvalidOperationException($"Type {responseType} is not a Result type.");
    }
}
