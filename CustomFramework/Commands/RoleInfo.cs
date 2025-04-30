using CommandSystem;
using CustomFramework.CustomSubclasses;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using System;

namespace CustomFramework.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    internal class RoleInfo : ICommand
    {
        public string Command => "roleinfo";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "See info about your role.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
            CustomSubclass role;
            response = "";

            if (player.IsAlive)
            {
                role = CustomSubclass.Get(player.UniqueRole);
            }
            else
            {
                SpectatorRole spectator = player.Role as SpectatorRole;
                Player spectated = spectator.SpectatedPlayer;

                role = CustomSubclass.Get(spectated.UniqueRole);
            }

            if (role != null)
            {
                response += $"\nDescription: {role.Description}\nInfo: {role.Info}";
                return true;
            }
            else
            {
                response = "User has no role.";
                return false;
            }
        }
    }
}
