using InventorySystem.Items.Usables.Scp330;
using LabApi.Events.Arguments.Interfaces;
using LabApi.Features.Wrappers;
namespace CustomFramework.EventArgs
{
	public class EatenCandyEventArgs : System.EventArgs, IPlayerEvent
	{
		public CandyKindID Kind { get; }

		public Player Player { get; }

		public EatenCandyEventArgs(CandyKindID kind, ReferenceHub hub)
		{
			Kind = kind;
			Player = Player.Get(hub);
		}
	}
}
