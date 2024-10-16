namespace SensitiveFileShare;

using Microsoft.Graph;
using System;
using System.Linq;
using System.Threading.Tasks;

public class TeamsHelper
{
    private readonly GraphServiceClient _graphClient;

    public TeamsHelper(GraphServiceClient graphClient)
    {
        _graphClient = graphClient;
    }

    // Fetch messages from Teams channels
    public async Task FetchMessagesFromChannelAsync(string teamId, string channelId)
    {
        var messages = await _graphClient.Teams[teamId].Channels[channelId].Messages
            .Request()
            .GetAsync();

        foreach (var message in messages.CurrentPage)
        {
            ProcessMessage(message);
        }
    }

    // Fetch messages from private and group chats
    public async Task FetchMessagesFromChatAsync(string chatId)
    {
        var messages = await _graphClient.Chats[chatId].Messages
            .Request()
            .GetAsync();

        foreach (var message in messages.CurrentPage)
        {
            ProcessMessage(message);
        }
    }

    private void ProcessMessage(ChatMessage message)
    {
        if (message.Attachments != null && message.Attachments.Any())
        {
            foreach (var attachment in message.Attachments)
            {
                if (attachment.ContentType == "application/vnd.microsoft.teams.file.download.info")
                {
                    Console.WriteLine($"File Shared: {attachment.Name} - URL: {attachment.ContentUrl}");

                    // Store file-sharing event in MySQL
                    StoreFileSharingEvent(attachment.Name, attachment.ContentUrl, message.From.User.DisplayName, message.CreatedDateTime.Value.DateTime);
                }
            }
        }
    }

    // Store event in MySQL database
    private void StoreFileSharingEvent(string fileName, string fileUrl, string sharedBy, DateTime sharedAt)
    {
        using (var connection = new MySql.Data.MySqlClient.MySqlConnection("Your MySQL Connection String"))
        {
            connection.Open();
            string query = "INSERT INTO file_sharing_events (file_name, file_url, shared_by, shared_at) VALUES (@fileName, @fileUrl, @sharedBy, @sharedAt)";
            using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@fileName", fileName);
                cmd.Parameters.AddWithValue("@fileUrl", fileUrl);
                cmd.Parameters.AddWithValue("@sharedBy", sharedBy);
                cmd.Parameters.AddWithValue("@sharedAt", sharedAt);
                cmd.ExecuteNonQuery();
            }
        }
    }
}