using CustomFramework.Enums;
using Exiled.API.Features;
using PlayerRoles;
using System.Reflection;

namespace CustomFramework.CustomSubclasses
{
    public static class Extensions
    {
        public static string GetTeam(this Player player)
        {
            var subclass = CustomSubclass.Get(player.UniqueRole);

            if (subclass != null)
            {
                var type = subclass.GetType();
                var attr = type.GetCustomAttribute<CustomSubclassAttribute>();
                return attr?.Team;
            }

            switch (player.Role.Type)
            {
                case PlayerRoles.RoleTypeId.ClassD:
                    return Teams.ClassD.ToString();
                case PlayerRoles.RoleTypeId.Scientist:
                    return Teams.Science.ToString();
                case PlayerRoles.RoleTypeId.FacilityGuard:
                    return Teams.FacilityGuard.ToString();
                case PlayerRoles.RoleTypeId.NtfPrivate:
                    return Teams.MTFPrivate.ToString();
                case PlayerRoles.RoleTypeId.NtfSpecialist:
                    return Teams.MTFSpecialist.ToString();
                case PlayerRoles.RoleTypeId.NtfSergeant:
                    return Teams.MTFSergeant.ToString();
                case PlayerRoles.RoleTypeId.NtfCaptain:
                    return Teams.MTFCaptain.ToString();
                case PlayerRoles.RoleTypeId.ChaosConscript:
                    return Teams.CIConscript.ToString();
                case PlayerRoles.RoleTypeId.ChaosRifleman:
                    return Teams.CIRifleman.ToString();
                case PlayerRoles.RoleTypeId.ChaosMarauder:
                    return Teams.CIMarauder.ToString();
                case PlayerRoles.RoleTypeId.ChaosRepressor:
                    return Teams.CIRepressor.ToString();
                case PlayerRoles.RoleTypeId.Scp049:
                    return Teams.SCP049.ToString();
                case PlayerRoles.RoleTypeId.Scp0492:
                    return Teams.SCP0492.ToString();
                case PlayerRoles.RoleTypeId.Scp079:
                    return Teams.SCP079.ToString();
                case PlayerRoles.RoleTypeId.Scp096:
                    return Teams.SCP096.ToString();
                case PlayerRoles.RoleTypeId.Scp106:
                    return Teams.SCP106.ToString();
                case PlayerRoles.RoleTypeId.Scp173:
                    return Teams.SCP173.ToString();
                case PlayerRoles.RoleTypeId.Scp939:
                    return Teams.SCP939.ToString();
                case PlayerRoles.RoleTypeId.Scp3114:
                    return Teams.SCP3114.ToString();
                case PlayerRoles.RoleTypeId.Flamingo:
                    return Teams.SCP1507.ToString();
                case PlayerRoles.RoleTypeId.AlphaFlamingo:
                    return Teams.SCP1507Alpha.ToString();
                case PlayerRoles.RoleTypeId.ZombieFlamingo:
                    return Teams.SCP1507Zombie.ToString();
                default:
                    return Teams.None.ToString();
            }
        }

        public static string GetTeam(this Faction faction)
        {
            switch (faction)
            {
                case Faction.SCP:
                    return Teams.SCP.ToString();
                case Faction.FoundationEnemy:
                    return Teams.ChaosInsurgency.ToString();
                case Faction.FoundationStaff:
                    return Teams.MobileTaskForce.ToString();
                case Faction.Flamingos:
                    return Teams.SCPFlamingo.ToString();
                default:
                    return Teams.None.ToString();
            }
        }
    }
}
