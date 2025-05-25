using CustomFramework.Enums;
using LabApi.Features.Wrappers;
using PlayerRoles;
using System.Reflection;

namespace CustomFramework.CustomSubclasses
{
    public static class Extensions
    {
        public static string GetTeam(this Player player)
        {
            var subclass = CustomSubclass.Get(CustomFrameworkPlugin.PlayerSubclasses[player]);

            if (subclass != null)
            {
                var type = subclass.GetType();
                var attr = type.GetCustomAttribute<CustomSubclassAttribute>();
                return attr?.Team;
            }

            switch (player.RoleBase.RoleTypeId)
            {
                case RoleTypeId.ClassD:
                    return Teams.ClassD.ToString();
                case RoleTypeId.Scientist:
                    return Teams.Science.ToString();
                case RoleTypeId.FacilityGuard:
                    return Teams.FacilityGuard.ToString();
                case RoleTypeId.NtfPrivate:
                    return Teams.MTFPrivate.ToString();
                case RoleTypeId.NtfSpecialist:
                    return Teams.MTFSpecialist.ToString();
                case RoleTypeId.NtfSergeant:
                    return Teams.MTFSergeant.ToString();
                case RoleTypeId.NtfCaptain:
                    return Teams.MTFCaptain.ToString();
                case RoleTypeId.ChaosConscript:
                    return Teams.CIConscript.ToString();
                case RoleTypeId.ChaosRifleman:
                    return Teams.CIRifleman.ToString();
                case RoleTypeId.ChaosMarauder:
                    return Teams.CIMarauder.ToString();
                case RoleTypeId.ChaosRepressor:
                    return Teams.CIRepressor.ToString();
                case RoleTypeId.Scp049:
                    return Teams.SCP049.ToString();
                case RoleTypeId.Scp0492:
                    return Teams.SCP0492.ToString();
                case RoleTypeId.Scp079:
                    return Teams.SCP079.ToString();
                case RoleTypeId.Scp096:
                    return Teams.SCP096.ToString();
                case RoleTypeId.Scp106:
                    return Teams.SCP106.ToString();
                case RoleTypeId.Scp173:
                    return Teams.SCP173.ToString();
                case RoleTypeId.Scp939:
                    return Teams.SCP939.ToString();
                case RoleTypeId.Scp3114:
                    return Teams.SCP3114.ToString();
                case RoleTypeId.Flamingo:
                    return Teams.SCP1507.ToString();
                case RoleTypeId.AlphaFlamingo:
                    return Teams.SCP1507Alpha.ToString();
                case RoleTypeId.ZombieFlamingo:
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
