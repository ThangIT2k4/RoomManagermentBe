using System.Text.Json;
using Identity.Domain.Common;

namespace Identity.API.Common;

public static class FilterParser
{
    /// <summary>
    /// Parse filter chuỗi (JSON) thành QueryFilter domain object.
    /// Nếu parse lỗi thì trả về null để tránh crash API.
    /// </summary>
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
            // Có thể log sau này, hiện tại chỉ bỏ qua filter không hợp lệ
            return null;
        }
    }
}

