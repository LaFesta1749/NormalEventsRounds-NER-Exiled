using Exiled.API.Features;
using System;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;

namespace NER
{
    public class NER : Plugin<Config>
    {
        public static NER Instance { get; private set; } = null!;

        public NER()
        {
            Instance = this;
        }
        public override string Name => "NormalEventsRounds";
        public override string Author => "LaFesta1749";
        public override string Prefix => "NER";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(9, 5, 1);

        private EventHandler? eventHandler;

        public bool IsRoundPaused { get; set; } = false;

        public void PauseRound()
        {
            IsRoundPaused = true;
            Log.Info("Event tracking has been paused for this round.");
        }

        public override void OnEnabled()
        {
            if (!Config.IsEnabled)
            {
                Log.Info("NormalEventsRounds is disabled in config.");
                return;
            }

            eventHandler = new EventHandler();
            Server.RoundEnded += eventHandler.OnRoundEnd;
            Player.Hurting += eventHandler.OnPlayerDamage;
            Player.Escaping += eventHandler.OnPlayerEscape;

            Log.Info("NormalEventsRounds has been enabled.");
        }

        public override void OnDisabled()
        {
            if (eventHandler != null)
            {
                Server.RoundEnded -= eventHandler.OnRoundEnd;
                Player.Hurting -= eventHandler.OnPlayerDamage;
                Player.Escaping -= eventHandler.OnPlayerEscape;
            }

            eventHandler = null!;
            Log.Info("NormalEventsRounds has been disabled.");
        }
    }
}
