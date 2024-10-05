using IdentityServer4.Models;
using System.Collections.Generic;

public static class ApiScopes
{
    public static IEnumerable<ApiScope> Get()
    {
        return new List<ApiScope>
        {
            new ApiScope("api1", "My API")
        };
    }
}
