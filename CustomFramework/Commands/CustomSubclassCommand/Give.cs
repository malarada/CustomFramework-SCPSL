using CommandSystem;
using CustomFramework.CustomSubclasses;
using LabApi.Features.Wrappers;
using System;

namespace CustomFramework.Commands.CustomSubclassCommand
{
    internal class Give : ICommand
    {
        public static Give Instance = new Give();

        public string Command => "give";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Give a subclass";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (arguments.Count == 0)
            {
                response = "give <SubclassId> [PlayerId|*]"; // [SetRole:true|false (default true)] [UseSpawnpoint:true|false (default false)] [ResetInventory:true|false (default false)]";
                return false;
            }

            CustomSubclass subclass = CustomSubclass.Get(int.Parse(arguments.At(0)));

            if (subclass == null)
            {
                response = "Invalid subclass.";
                return false;
            }

            var person = arguments.Count == 1 ? player.PlayerId.ToString() : arguments.At(1);

            if (int.TryParse(person, out var p))
            {
                if (Player.Get(p) == player && !sender.CheckPermission(PlayerPermissions.ForceclassSelf) && !sender.CheckPermission(PlayerPermissions.ForceclassWithoutRestrictions))
                {
                    response = "Missing permission: requires ForceclassSelf OR ForceclassWithoutRestrictions.";
                    return false;
                }
                if (!sender.CheckPermission(PlayerPermissions.ForceclassWithoutRestrictions))
                {
                    response = "Missing permission: requires ForceclassWithoutRestrictions.";
                    return false;
                }

                if (CustomFrameworkPlugin.PlayerSubclasses[Player.Get(p)] != null && CustomFrameworkPlugin.PlayerSubclasses[Player.Get(p)] != "")
                    CustomSubclass.Get(CustomFrameworkPlugin.PlayerSubclasses[Player.Get(p)]).RemoveSubclass(Player.Get(p));

                subclass.GiveSubclass(Player.Get(p));
                response = "Subclass given to player.";
                return true;
            }
            else if (person == "*")
            {
                foreach (Player ply in Player.List)
                {
                    if (CustomFrameworkPlugin.PlayerSubclasses[ply] != null && CustomFrameworkPlugin.PlayerSubclasses[ply] != "")
                        CustomSubclass.Get(CustomFrameworkPlugin.PlayerSubclasses[ply]).RemoveSubclass(ply);
                    subclass.GiveSubclass(ply);
                }
                response = "Subclass given to all players.";
                return true;
            }
            else
            {
                response = "Invalid player.";
                return false;
            }
        }
    }
}
