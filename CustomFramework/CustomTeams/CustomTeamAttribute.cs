using CustomFramework.Enums;

namespace CustomFramework.CustomTeams
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    internal class CustomTeamAttribute : System.Attribute
    {
        public Teams TeamToReplace { get; set; }

        public CustomTeamAttribute(Teams team)
        {
            TeamToReplace = team;
        }
    }
}
