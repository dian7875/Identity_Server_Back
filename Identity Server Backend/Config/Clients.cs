using IdentityServer4.Models;

public static class Clients
{
    public static IEnumerable<Client> Get()
    {
        return new List<Client>
        {
            new Client
            {
                ClientId = "client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                AllowedScopes = { "api1" }
            },
            new Client
        {
      ClientId = "Dashboard",
        AllowedGrantTypes = GrantTypes.Code,
        ClientSecrets = { new Secret("secret".Sha256()) },
        RedirectUris = { "https://tu-dashboard.com/signin-oidc" },
        PostLogoutRedirectUris = { "https://tu-dashboard.com/signout-callback-oidc" }, 
        AllowedScopes = { "openid", "profile", "api1" },
        RequireConsent = false, 
        AllowOfflineAccess = true, 
        RequirePkce = true 
},
            new Client
            { 
            ClientId = "Fronted1",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
             ClientSecrets =
             {
               new Secret("secret".Sha256())
              },
             AllowedScopes = { "api1" },
             RedirectUris = { "https://zcz17ld0-5173.use2.devtunnels.ms" } // Aquí agregas la URL de redirección
} };
            
    }
}