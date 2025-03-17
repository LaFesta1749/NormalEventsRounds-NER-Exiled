using Exiled.API.Features;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

namespace NER
{
    public static class WinnersManager
    {
        private static readonly string FilePath = Path.Combine(Paths.Configs, "winners.yml");
        private static Dictionary<string, int> winners = new();

        static WinnersManager()
        {
            LoadWinners();
        }

        public static void AddWin(string userId)
        {
            if (!winners.ContainsKey(userId))
                winners[userId] = 0;

            winners[userId]++;
            SaveWinners();
        }

        public static List<string> GetTopPlayers(int count)
        {
            return winners
                .OrderByDescending(x => x.Value)
                .Take(count)
                .Select(x => {
                    Player player = Player.Get(x.Key);
                    string playerName = player != null ? player.Nickname : x.Key; // Ако не може да намери играча, връща Steam ID
                    return $"🏅 **{playerName}** - {x.Value} wins";
                })
                .ToList();
        }

        public static int GetWins(string userId)
        {
            return winners.TryGetValue(userId, out int wins) ? wins : 0;
        }

        public static void ResetWinners()
        {
            winners.Clear();
            SaveWinners();
            Log.Info("All winners have been reset.");
        }

        private static void LoadWinners()
        {
            if (!File.Exists(FilePath))
            {
                SaveWinners();
                return;
            }

            try
            {
                string yamlContent = File.ReadAllText(FilePath);
                winners = new Deserializer().Deserialize<Dictionary<string, int>>(yamlContent) ?? new();
            }
            catch
            {
                Log.Error("Failed to load winners.yml. Resetting data.");
                winners = new();
                SaveWinners();
            }
        }

        private static void SaveWinners()
        {
            try
            {
                string yamlContent = new Serializer().Serialize(winners);
                File.WriteAllText(FilePath, yamlContent);
            }
            catch
            {
                Log.Error("Failed to save winners.yml.");
            }
        }
    }
}
