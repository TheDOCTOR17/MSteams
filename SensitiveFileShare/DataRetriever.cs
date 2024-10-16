namespace SensitiveFileShare;

using Microsoft.Graph;
public class DataRetriever
{
    private readonly GraphServiceClient _graphClient;

    public DataRetriever(GraphServiceClient graphClient)
    {
        _graphClient = graphClient;
    }

    // Fetch all Teams and Channels
    public async Task FetchAllTeamsAndChannelsAsync(TeamsHelper teamsHelper)
    {
        var teams = await _graphClient.Groups
            .Request()
            .Filter("resourceProvisioningOptions/Any(x:x eq 'Team')")
            .GetAsync();

        foreach (var team in teams.CurrentPage)
        {
            var channels = await _graphClient.Teams[team.Id].Channels
                .Request()
                .GetAsync();

            foreach (var channel in channels.CurrentPage)
            {
                await teamsHelper.FetchMessagesFromChannelAsync(team.Id, channel.Id);
            }
        }
    }

    // Fetch all 1:1 and group chats
    public async Task FetchAllChatsAsync(TeamsHelper teamsHelper)
    {
        var chats = await _graphClient.Me.Chats
            .Request()
            .GetAsync();

        foreach (var chat in chats.CurrentPage)
        {
            await teamsHelper.FetchMessagesFromChatAsync(chat.Id);
        }
    }
}