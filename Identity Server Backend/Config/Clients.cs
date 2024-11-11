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