using CommandSystem;
using Exiled.API.Features;
using NER;
using System;

namespace NER
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class NERCommand : ICommand
    {
        public string Command => "ner";
        public string[] Aliases => new[] { "nerclear", "nerpause" };
        public string Description => "Commands to manage Normal Events Rounds (NER) plugin.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response = "Usage: ner_reset | ner_pause";
                return false;
            }

            switch (arguments.At(0).ToLower())
            {
                case "reset":
                    WinnersManager.ResetWinners();
                    response = "All event winners have been reset.";
                    return true;

                case "pause":
                    NER.Instance.PauseRound();
                    response = "Event tracking is now paused for this round.";
                    return true;

                default:
                    response = "Invalid subcommand. Usage: ner_reset | ner_pause";
                    return false;
            }
        }
    }
}
