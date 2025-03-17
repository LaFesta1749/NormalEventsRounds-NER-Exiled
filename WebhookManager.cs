using Exiled.API.Features;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NER
{
    public static class WebhookManager
    {
        private static readonly HttpClient HttpClient = new();

        public static async void SendWinNotification(string userId, int wins, string eventName)
        {
            string webhookUrl = NER.Instance.Config.WebhookUrl;
            if (string.IsNullOrEmpty(webhookUrl))
            {
                Log.Warn("Webhook URL is not set in the config!");
                return;
            }

            Player player = Player.Get(userId);
            if (player == null)
                return;

            // Извличане на топ 3 следващи играчи
            var topPlayers = WinnersManager.GetTopPlayers(3);
            string topPlayersText = topPlayers.Count > 0 ? string.Join("\n", topPlayers) : "No other players yet.";

            var embed = new
            {
                title = $"🎉 [EVENT WINNER] - {eventName}",
                description = $"**{player.Nickname}** won the event: **{eventName}**!\n🏆 **{player.Nickname}** now has **{wins}/{NER.Instance.Config.WinsRequired}** wins!",
                color = 16753920, // Orange color
                fields = new[]
                {
            new { name = "📊 Next Top 3 Players:", value = topPlayersText, inline = false }
        },
                footer = new { text = "SCP:SL PARLAMATA Event System" }
            };

            var payload = new
            {
                username = "Event System",
                embeds = new[] { embed }
            };

            // Проверка дали играчът е достигнал максималните победи
            if (wins >= NER.Instance.Config.WinsRequired)
            {
                SendRewardNotification(userId); // Изпраща отделен embed за наградата
            }
            else
            {
                await SendWebhook(webhookUrl, payload);
            }
        }

        public static async void SendRewardNotification(string userId)
        {
            string webhookUrl = NER.Instance.Config.WebhookUrl;
            if (string.IsNullOrEmpty(webhookUrl))
            {
                Log.Warn("Webhook URL is not set in the config!");
                return;
            }

            Player player = Player.Get(userId);
            if (player == null)
                return;

            var embed = new
            {
                title = "🎁 [EVENT]",
                description = $"**{player.Nickname}** has reached **{NER.Instance.Config.WinsRequired} wins** and should receive a reward!\n\n⚠ Webhook notifications are paused until the winners have been cleared.",
                color = 65280, // Green color
                footer = new { text = "SCP:SL PARLAMATA Event System" }
            };

            var payload = new
            {
                username = "Event System",
                content = "<@902836984102395915> <@294860832297320458>", // Ping admin Discord IDs
                embeds = new[] { embed }
            };

            await SendWebhook(webhookUrl, payload);
        }

        private static async Task SendWebhook(string url, object payload)
        {
            try
            {
                string json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await HttpClient.PostAsync(url, content);
            }
            catch
            {
                Log.Error("Failed to send webhook notification.");
            }
        }
    }
}
