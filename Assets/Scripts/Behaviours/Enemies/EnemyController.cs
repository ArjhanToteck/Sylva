using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public EnemyData data;
	public bool dying = false;

	[Header("Local Components")]
	public Animator animator;
	public CharacterController2D controller;
	public GameObject exclamationPoint;
	public Rigidbody2D rigidbody;
	public ParticleSystem particleSystem;

	[Header("Player")]
	public GameObject player;
	public bool playerFound = false;

	public List<Action> onDeathEvents = new List<Action>();

	void OnTriggerStay2D(Collider2D collider)
	{
		if (dying) return;

		// checks if player is in vision
		if (collider.CompareTag("Player"))
		{
			OnPlayerFound(collider);
		}

		// checks if hit player
		if (collider.CompareTag("PlayerHit"))
		{
			Hit(collider);
		}
	}

	void FixedUpdate()
	{
		// checks if going towards player
		if (playerFound && !dying && controller.grounded)
		{
			// moves towards player
			float movement = data.speed * Time.deltaTime;

			if (transform.position.x > player.transform.position.x) movement *= -1f;

			controller.Move(movement, false, false);
		}
	}

	void OnParticleCollision(GameObject collider)
	{
		if (!!collider.GetComponent<ParticleCollisionManager>())
		{
			collider.GetComponent<ParticleCollisionManager>().TriggerParticleCollision(gameObject);
		}
	}

	public void OnPlayerFound(Collider2D collider)
	{
		// marks player as found
		playerFound = true;
		player = collider.gameObject;

		// shows exclamation point
		exclamationPoint.SetActive(true);
	}

	public void Hit(Collider2D collider)
	{
		Vector2 knockback = data.knockback;

		if(collider.transform.position.x < transform.position.x) knockback.x *= -1f;

		collider.transform.parent.GetComponent<PlayerController>().TakeHit(data.damage, knockback);
	}

	public void TakeHit(int damage, Vector2 knockback, bool physicalHit = true)
	{
		// plays particle system
		if(!!particleSystem && physicalHit) particleSystem.Play();

		// takes damage
		data.health -= damage;

		// checks if health is below 0
		if (data.health <= 0)
		{
			Die();
		}
		else
		{
			// only applies knockback if not dead
			rigidbody.velocity = new Vector2(rigidbody.velocity.x, knockback.y);
			controller.Move(knockback.x, false, false);
		}
	}

	public void Die()
	{
		// calls onDeath events
		foreach(Action action in onDeathEvents)
		{
			action();
		}

		// freezes position and rotation while dying
		rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;

		// marks as dying
		dying = true;

		// plays dying animation
		animator.SetTrigger("Die");
	}

	public void DoneDying()
	{
		Destroy(gameObject);
	}
}
