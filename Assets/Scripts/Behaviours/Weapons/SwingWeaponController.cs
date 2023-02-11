using System;
using System.Collections.Generic;
using UnityEngine;

public class SwingWeaponController : MonoBehaviour
{
	public Item.Weapon weapon;
	public PlayerController playerController;
	public BoxCollider2D collider;
	public SpriteRenderer renderer;
	public Animator animator;

	public List<SpriteRenderer> armRenderers = new List<SpriteRenderer>();

	// events for controller children
	public List<Action<Collider2D>> onHitEvents = new List<Action<Collider2D>>();
	public List<Action<Collider2D>> onHitEnemyEvents = new List<Action<Collider2D>>();
	public List<Action> onSwingEvents = new List<Action>();
	public List<Action> onSwingFinishEvents = new List<Action>();

	void OnEnable()
	{
		// gets weapon data
		weapon = playerController.weaponSelectedBeforeAttack;

		// sets sprite
		renderer.sprite = weapon.sprite;

		// sets collider size to diagonal length of weapon
		collider.size = new Vector2(weapon.sprite.bounds.extents.x * Mathf.Sqrt(2) + 0.5f, weapon.sprite.bounds.extents.x * Mathf.Sqrt(2) + 0.5f);

		// positions weapon correctly
		collider.offset = new Vector2((weapon.sprite.bounds.extents.x * Mathf.Sqrt(2) + 1) / 2, (weapon.sprite.bounds.extents.x * Mathf.Sqrt(2) + 1) / 2);
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		CollisionHandler(true, collider);
	}

	void CollisionHandler(bool trigger, Collider2D collider)
	{
		// calls all events for onHit
		foreach (Action<Collider2D> action in onHitEvents)
		{
			action(collider);
		}

		// checks if enemy
		if (collider.CompareTag("Enemy"))
		{
			weapon.OnHit(gameObject, collider);

			// calls all events for onHitEnemy
			foreach (Action<Collider2D> action in onHitEnemyEvents)
			{
				action(collider);
			}
		}
	}

	void OnDestroy()
	{
		OnDisable();
	}

	void OnDisable()
	{
		// checks if swing attack is finished
		if (playerController.swingAttackProgress > 0.99f)
		{
			// resets progress to -1 to show that its finished
			playerController.swingAttackProgress = -1f;

			// calls all events for onSwingFinish
			foreach (Action action in onSwingFinishEvents)
			{
				action();
			}
		}
		else
		{
			// this makes it so even if a player starts running while attacking, the weapon does not go away
			playerController.swingAttack = true;
		}

		playerController.inSwingAttackAnimation = false;
	}

	public void TriggerOnSwingEvents()
	{
		// calls all events for onSwing
		foreach (Action action in onSwingEvents)
		{
			action();
		}
	}

	public void ClearEvents()
	{
		onHitEvents.Clear();
		onHitEnemyEvents.Clear();
		onSwingEvents.Clear();
		onSwingFinishEvents.Clear();
	}
}
