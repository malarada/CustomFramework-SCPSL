using CustomFramework.CustomSubclasses;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using System.Collections.Generic;
using Handlers = Exiled.Events.Handlers;
using MEC;
using Exiled.Events.EventArgs.Server;
using System.Linq;

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
		public CustomSubclass PreviousSubclass { get; set; } = null;
		public CoroutineHandle coroutine { get; set; }

		public override void SubscribeEvents()
		{
			base.SubscribeEvents();

			Handlers.Player.Died += Died;
			Handlers.Player.Handcuffing += Handcuffing;

			Handlers.Server.EndingRound += RoundEnding;
		}

		public override void UnsubscribeEvents()
		{
			base.UnsubscribeEvents();

			Handlers.Player.Died -= Died;
			Handlers.Player.Handcuffing -= Handcuffing;

			Handlers.Server.EndingRound -= RoundEnding;
		}

		private void Handcuffing(HandcuffingEventArgs ev)
		{
			if (Check(ev.Target))
			{
				ev.IsAllowed = false;
				ev.Player.EnableEffect(Exiled.API.Enums.EffectType.Slowness, 50, 2f);
				ev.Player.EnableEffect(Exiled.API.Enums.EffectType.Concussed, 2f);
			}
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
				ev.Player.Role.Set(RoleTypeId.Tutorial);
				PreviousSubclass = Get(ev.Player.UniqueRole);
				GiveSubclass(ev.Player);

				ChaosAgent = ev.Player;
			}
		}

		public override void GiveSubclass(Player player)
		{
			PreviousSubclass = Get(player.UniqueRole);
			PreviousSubclass.RemoveSubclass(player);

			base.GiveSubclass(player);

			player.AddItem(ItemType.GunSCP127);
			player.AddItem(ItemType.GunFRMG0);
			player.AddItem(ItemType.Jailbird);
			player.AddItem(ItemType.ArmorHeavy);
			player.AddItem(ItemType.KeycardO5);
			player.AddItem(ItemType.Radio);
			player.AddAmmo(Exiled.API.Enums.AmmoType.Nato556, 200);

			player.MaxHealth = 3500;
			player.Health = player.MaxHealth;

			coroutine = Timing.RunCoroutine(Coroutine());

			PreviousSubclass.GiveSubclass(player);

			player.UniqueRole = Identifier;
		}

		public override void RemoveSubclass(Player player)
		{
			base.RemoveSubclass(player);

			PreviousSubclass.RemoveSubclass(player);
		}

		public override string GetSpecificHint(Player player) => PreviousSubclass.GetSpecificHint(player);

		public override void OnAbility(Player player) => PreviousSubclass.OnAbility(player);

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

			foreach (var item in new List<RoleTypeId>()
			{
				RoleTypeId.ClassD,
				RoleTypeId.Scientist,
				RoleTypeId.FacilityGuard,
				RoleTypeId.NtfCaptain,
				RoleTypeId.ChaosRepressor,
				RoleTypeId.Scp049,
				RoleTypeId.Scp0492,
				RoleTypeId.Scp079,
				RoleTypeId.Scp096,
				RoleTypeId.Scp106,
				RoleTypeId.Scp173,
				RoleTypeId.Scp939,
				RoleTypeId.Scp3114
			})
			{
				if (Player.List.Any(t => t.Role == item))
				{
					CATargets.Add(item);
				}
			}
		}

		private string GetChaosAgentTarget()
		{



			return null;
		}
	}
}
