using CustomFramework.CustomSubclasses;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using Handlers = Exiled.Events.Handlers;
using MEC;
using Exiled.Events.EventArgs.Server;

namespace Chaos_Agent
{
	internal class ChaosAgentSubclass : CustomSubclass
	{
		public override string Identifier { get; set; } = "ChaosAgent";
		public override string Name { get; set; } = "Chaos Agent";
		public override float SpawnTickets { get; set; } = 0;
		public override string Description { get; set; } = "You are the Chaos Agent. Your targets will be communicated via C.A.S.S.I.E.";
		public override string Info { get; set; } = "The goal of the Chaos Agent is to kill their target group before the end of the round. The target group may change.";
		public override string CustomInfo { get; set; } = "Chaos Agent";
		public Player ChaosAgent { get; set; } = null;
		public CoroutineHandle coroutine { get; set; }

		public override void SubscribeEvents()
		{
			base.SubscribeEvents();

			Handlers.Player.Died += Died;

			Handlers.Server.EndingRound += RoundEnding;
		}

		public override void UnsubscribeEvents()
		{
			base.UnsubscribeEvents();

			Handlers.Player.Died -= Died;

			Handlers.Server.EndingRound -= RoundEnding;
		}

		private void Died(DiedEventArgs ev)
		{
			if (ChaosAgent != null)
			{
				if (Check(ev.Player))
				{
					Cassie.MessageTranslated("Chaos Agent . Terminated", "Chaos Agent; Terminated.");
				}
			}
			else
			{
				ChaosAgent = ev.Player;
			}
		}

		public override void GiveSubclass(Player player)
		{
			base.GiveSubclass(player);

			player.AddItem(ItemType.GunSCP127);
			player.AddItem(ItemType.GunFRMG0);
			player.AddItem(ItemType.Jailbird);
			player.AddItem(ItemType.ArmorHeavy);
			player.AddItem(ItemType.KeycardO5);
			player.AddItem(ItemType.Radio);
			player.AddItem(ItemType.AntiSCP207);
			player.AddAmmo(Exiled.API.Enums.AmmoType.Nato556, 200);

			player.MaxHealth = 250 * (Player.List.Count + Player.Get(Team.SCPs).Count());
			player.Health = player.MaxHealth;

			coroutine = Timing.RunCoroutine(Coroutine());
		}

		private void RoundEnding(EndingRoundEventArgs ev)
		{
			if (coroutine != null && coroutine.IsRunning)
				Timing.KillCoroutines(coroutine);
		}

		private RoleTypeId ChaosAgentTarget { get; set; } = RoleTypeId.None;

		public IEnumerator<float> Coroutine()
		{
			while (true)
			{


				yield return Timing.WaitForSeconds(120f);
			}
		}

		private void ChangeChaosAgentTarget()
		{
			List<RoleTypeId> CATargets = new List<RoleTypeId>();
			//{
			//	RoleTypeId.ClassD,
			//	RoleTypeId.Scientist,
			//	RoleTypeId.FacilityGuard,
			//	RoleTypeId.NtfCaptain,
			//	RoleTypeId.ChaosRepressor,
			//	RoleTypeId.Scp049,
			//	RoleTypeId.Scp0492,
			//	RoleTypeId.Scp079,
			//	RoleTypeId.Scp096,
			//	RoleTypeId.Scp106,
			//	RoleTypeId.Scp173,
			//	RoleTypeId.Scp939,
			//	RoleTypeId.Scp3114
			//};


		}

		private string GetChaosAgentTarget()
		{



			return null;
		}
	}
}
