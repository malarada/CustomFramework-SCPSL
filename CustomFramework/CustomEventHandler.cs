using CustomFramework.EventArgs;
using LabApi.Events;

namespace CustomFramework
{
	public class CustomEventHandler
	{
		public static event LabEventHandler<EatenCandyEventArgs> EatenCandy;

		public static void OnEatenCandy(EatenCandyEventArgs ev)
		{
			EatenCandy?.Invoke(ev);
		}
	}
}
