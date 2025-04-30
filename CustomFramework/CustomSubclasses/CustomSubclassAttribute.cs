using CustomFramework.Enums;

namespace CustomFramework.CustomSubclasses
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class CustomSubclassAttribute : System.Attribute
    {
        public string Team { get; set; }

        public CustomSubclassAttribute(Teams team) : this(team.ToString())
        {
        }

        public CustomSubclassAttribute(string team)
        {
            Team = team;
        }
    }
}
