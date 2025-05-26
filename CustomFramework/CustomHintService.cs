using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;

namespace CustomFramework
{
	public class CustomHintService
	{
		internal static List<Func<Player, string>> hints = new List<Func<Player, string>>();

		public static void RegisterHint(Func<Player, string> hint)
		{
			if (hint == null)
				throw new ArgumentNullException(nameof(hint), "Hint cannot be null.");

			hints.Add(hint);
		}
	}
}
