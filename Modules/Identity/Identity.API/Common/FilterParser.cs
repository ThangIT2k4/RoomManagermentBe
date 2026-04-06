using System.Text.Json;
using Identity.Domain.Common;

namespace Identity.API.Common;

public static class FilterParser
{
    public static QueryFilter? Parse(string? filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<QueryFilter>(filter, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            return null;
        }
    }
}

