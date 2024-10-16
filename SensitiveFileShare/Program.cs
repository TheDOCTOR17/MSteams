// See https://aka.ms/new-console-template for more information
namespace SensitiveFileShare;

using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    private static async Task Main(string[] args)
    {
        // Initialize Microsoft Graph Client
        await GraphHelper.InitializeGraphClient("client-id", "tenant-id", "client-secret");
        var graphClient = GraphHelper.GetAuthenticatedClient();

        var teamsHelper = new TeamsHelper(graphClient);
        var dataRetriever = new DataRetriever(graphClient);

        // Poll every 5 minutes
        while (true)
        {
            // Fetch and process messages from all teams and channels
            await dataRetriever.FetchAllTeamsAndChannelsAsync(teamsHelper);

            // Fetch and process messages from all 1:1 and group chats
            await dataRetriever.FetchAllChatsAsync(teamsHelper);

            Console.WriteLine("Checked for new messages. Waiting for 5 minutes...");
            Thread.Sleep(TimeSpan.FromMinutes(5));  // Wait 5 minutes before polling again
        }
    }
}
