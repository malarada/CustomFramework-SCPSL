using CommandSystem;
using CustomFramework.CustomSubclasses;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomFramework.Commands.CustomSubclassCommand
{
    internal class List : ICommand
    {
        public static List Instance = new List();

        public string Command => "list";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Get a list of custom subclasses.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (!player.CheckPermission(PlayerPermissions.ForceclassSelf) && !player.CheckPermission(PlayerPermissions.ForceclassWithoutRestrictions))
            {
                response = "You do not have permission to use this command (Need permission: ForceclassSelf OR ForceclassWithoutResrictions)";
                return false;
            }

            response = "Custom Subclasses:";

            foreach (var role in CustomSubclass.Registered)
            {
                var roleAttr = role.GetType().GetCustomAttribute<CustomSubclassAttribute>();
                if (roleAttr != null)
                    response += $"\n[Id: {role.Id}, Identifier: {role.Identifier}, Team: {roleAttr.Team}";
            }

            return true;
        }
    }
}
