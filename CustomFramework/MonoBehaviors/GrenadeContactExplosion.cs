using GameCore;
using InventorySystem.Items.ThrowableProjectiles;
using System;
using UnityEngine;

namespace CustomFramework.MonoBehaviors
{
	public class GrenadeContactExplosion : MonoBehaviour
	{
		private bool initialized;

		/// <summary>
		/// Gets the thrower of the grenade.
		/// </summary>
		public GameObject Owner { get; private set; }

		/// <summary>
		/// Gets the grenade itself.
		/// </summary>
		public EffectGrenade Grenade { get; private set; }

		/// <summary>
		/// Inits the <see cref="CollisionHandler"/> object.
		/// </summary>
		/// <param name="owner">The grenade owner.</param>
		/// <param name="grenade">The grenade component.</param>
		public void Init(GameObject owner, ThrownProjectile grenade)
		{
			Owner = owner;
			Grenade = (EffectGrenade)grenade;
			initialized = true;
		}

		private void OnCollisionEnter(Collision collision)
		{
			try
			{
				if (!initialized)
					return;
				if (Owner == null)
					LabApi.Features.Console.Logger.Error($"Owner is null!");
				if (Grenade == null)
					LabApi.Features.Console.Logger.Error("Grenade is null!");
				if (collision is null)
					LabApi.Features.Console.Logger.Error("wat");
				if (!collision.collider)
					LabApi.Features.Console.Logger.Error("water");
				if (collision.collider.gameObject == null)
					LabApi.Features.Console.Logger.Error("pepehm");
				if (collision.collider.gameObject == Owner || collision.collider.gameObject.TryGetComponent<EffectGrenade>(out _))
					return;

				Grenade.TargetTime = 0.1f;
			}
			catch (Exception exception)
			{
				LabApi.Features.Console.Logger.Error($"{nameof(OnCollisionEnter)} error:\n{exception}");
				Destroy(this);
			}
		}
	}
}
