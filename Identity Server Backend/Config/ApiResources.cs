using IdentityServer4.Models;
using System.Collections.Generic;

public static class ApiResources
{
    public static IEnumerable<ApiResource> Get()
    {
        return new List<ApiResource>
        {
            new ApiResource("api1", "My API")
        };
    }
}
