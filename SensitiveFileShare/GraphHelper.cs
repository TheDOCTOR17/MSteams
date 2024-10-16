namespace SensitiveFileShare;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public class GraphHelper
{
    private static GraphServiceClient _graphClient;

    public static async Task InitializeGraphClient(string clientId, string tenantId, string clientSecret)
    {
        var confidentialClientApp = ConfidentialClientApplicationBuilder
            .Create(clientId)
            .WithClientSecret(clientSecret)
            .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
            .Build();

        var authProvider = new ClientCredentialProvider(confidentialClientApp);
        _graphClient = new GraphServiceClient(authProvider);
    }

    public static GraphServiceClient GetAuthenticatedClient()
    {
        return _graphClient;
    }
}