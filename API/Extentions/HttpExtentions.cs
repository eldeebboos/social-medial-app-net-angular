using System;
using System.Text.Json;
using API.Helpers;

namespace API.Extentions;

public static class HttpExtentions
{
    public static void AddPageinationHeader<T>(this HttpResponse response, PagedList<T> data)
    {
        var pageinationHeader = new PageinationHeader(data.CurrentPage, data.PageSize,
                                        data.TotalCount,
                                        data.TotalPages);

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        response.Headers.Append("Pageination", JsonSerializer.Serialize(pageinationHeader, jsonOptions));
        response.Headers.Append("Access-Control-Expose-Headers", "Pageination");
        
    }
}
