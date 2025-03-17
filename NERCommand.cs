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
        public string Command => "ner_reset";
        public string[] Aliases => new[] { "nerclear" };
        public string Description => "Resets all event winners and restarts the event tracking.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            WinnersManager.ResetWinners();
            response = "All event winners have been reset.";
            return true;
        }
    }
}
