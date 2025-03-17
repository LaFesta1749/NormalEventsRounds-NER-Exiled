using Exiled.API.Interfaces;
using System.ComponentModel;

namespace NER
{
    public class Config : IConfig
    {
        [Description("Enable or disable the plugin.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Enable or disable debug logs.")]
        public bool Debug { get; set; } = false;

        [Description("Webhook URL for event notifications.")]
        public string WebhookUrl { get; set; } = "https://your-webhook-url-here";

        [Description("Number of wins required for a reward.")]
        public int WinsRequired { get; set; } = 3;
    }
}
