using Exiled.API.Features;
using PlayerRoles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using System.Collections.Generic;
using System.Linq;

namespace NER
{
    public class EventHandler
    {
        private Dictionary<string, float> scpDamageTracker = new();
        private string? firstEscapedPlayer = null;

        public void AnnounceWinner(string winnerName, string eventName)
        {
            if (!NER.Instance.Config.EnableBroadcasts)
                return;

            string message = $"<color=yellow>[PARLAMATA EVENTS]</color> <color=green>{winnerName}</color> won <color=red>{eventName}</color>!";
            Map.Broadcast(10, message); // Показва съобщението за 10 секунди
        }

        public void OnPlayerDamage(HurtingEventArgs ev)
        {
            // Проверяваме дали целта е SCP
            if (ev.Attacker != null && ev.Player.Role.Side == Exiled.API.Enums.Side.Scp)
            {
                if (!scpDamageTracker.ContainsKey(ev.Attacker.UserId))
                    scpDamageTracker[ev.Attacker.UserId] = 0;

                scpDamageTracker[ev.Attacker.UserId] += ev.Amount;

                if (NER.Instance.Config.Debug)
                    Log.Info($"{ev.Attacker.Nickname} dealt {ev.Amount} damage to {ev.Player.Role} (Total: {scpDamageTracker[ev.Attacker.UserId]})");
            }
        }

        public void OnPlayerEscape(EscapingEventArgs ev)
        {
            // Проверяваме дали играчът е първият избягал (Class-D или Scientist)
            if (firstEscapedPlayer == null && (ev.Player.Role.Type == RoleTypeId.ClassD || ev.Player.Role.Type == RoleTypeId.Scientist))
            {
                firstEscapedPlayer = ev.Player.UserId;
                if (NER.Instance.Config.Debug)
                    Log.Info($"{ev.Player.Nickname} was the first to escape!");
            }
        }

        public void OnRoundEnd(RoundEndedEventArgs ev)
        {
            // Проверяваме дали текущият рунд е паузиран
            if (NER.Instance.IsRoundPaused)
            {
                Log.Info("Round was paused. No event winners recorded.");
                NER.Instance.IsRoundPaused = false; // Автоматично активиране на събитията за следващия рунд
                return;
            }

            string winner = DetermineWinner();

            if (!string.IsNullOrEmpty(winner))
            {
                string winnerName = Player.Get(winner)?.Nickname ?? "Unknown";
                string eventName = GetEventName(winner);

                WinnersManager.AddWin(winner);
                WebhookManager.SendWinNotification(winner, WinnersManager.GetWins(winner), eventName);
                AnnounceWinner(winnerName, eventName); // Изпращаме съобщение в играта
            }

            // Нулираме тракерите за следващия рунд
            scpDamageTracker.Clear();
            firstEscapedPlayer = null;
        }

        private string GetEventName(string winnerId)
        {
            if (Player.List.Count(p => p.IsAlive) == 1)
                return "Last Man Standing";

            if (scpDamageTracker.ContainsKey(winnerId) && scpDamageTracker[winnerId] > 0)
                return "SCP Hunter";

            if (firstEscapedPlayer == winnerId)
                return "First Escape";

            return "Unknown Event";
        }

        private string DetermineWinner()
        {
            // 1. Проверяваме за Last Man Standing
            var alivePlayers = Player.List.Where(p => p.IsAlive).ToList();
            if (alivePlayers.Count == 1)
            {
                if (NER.Instance.Config.Debug)
                    Log.Info($"Winner by Last Man Standing: {alivePlayers[0].Nickname}");
                return alivePlayers[0].UserId;
            }

            // 2. Проверяваме за SCP Hunter
            if (scpDamageTracker.Count > 0)
            {
                var topDamager = scpDamageTracker.OrderByDescending(d => d.Value).FirstOrDefault();
                if (NER.Instance.Config.Debug)
                    Log.Info($"Winner by SCP Hunter: {Player.Get(topDamager.Key)?.Nickname} with {topDamager.Value} damage.");
                return topDamager.Key;
            }

            // 3. Проверяваме за First Escape
            if (firstEscapedPlayer != null)
            {
                if (NER.Instance.Config.Debug)
                    Log.Info($"Winner by First Escape: {Player.Get(firstEscapedPlayer)?.Nickname}");
                return firstEscapedPlayer;
            }

            return string.Empty;
        }
    }
}
