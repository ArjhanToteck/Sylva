using System;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using Random = UnityEngine.Random;

public class AcidSprayController : MonoBehaviour
{
	public SpellController spellController;
	public GameObject acid;
	public ParticleSystem acidParticleSystem;

	int damage = 1;
	Vector2 knockback = new Vector2(5, 8);

	public float hSliderValue = 1.0F;

	void Start()
	{
		// loads spellController
		spellController = transform.parent.Find("SpellController").GetComponent<SpellController>();

		// clones acid so it isn't deleted when spell is unequipped
		acid = Instantiate(acid);
		acidParticleSystem = acid.GetComponent<ParticleSystem>();

		// damages enemy when hit
		acid.GetComponent<ParticleCollisionManager>().onParticleCollisionEvents.Add(CollisionHandler);

		// sprays acid when spell is cast
		spellController.onCastEvents.Add(new Action(() =>
		{
			// flips player if aiming opposite direction

			// aiming forwards, but player facing backwards
			if((spellController.playerController.aimAngle > 280 || spellController.playerController.aimAngle < 80) && spellController.playerController.transform.localScale.x < 0)
			{
				spellController.playerController.Flip();
			}
			// aiming backwardss, but player facing forwards
			else if((spellController.playerController.aimAngle < 260 && spellController.playerController.aimAngle > 100) && spellController.playerController.transform.localScale.x > 0)
			{
				spellController.playerController.Flip();
			}

			// aims particles

			// direction of entire system
			acid.transform.rotation = Quaternion.Euler(new Vector3(0, 0, spellController.playerController.aimAngle));

			// direction of particles
			MainModule main = acidParticleSystem.main;
			main.startRotation = Mathf.Deg2Rad * (-spellController.playerController.aimAngle + 90);

			// inverts rotation over time if facing backwards
			if (spellController.playerController.transform.localScale.x < 0)
			{
				main.flipRotation = 1;
				main.startRotation = Mathf.Deg2Rad * -(-spellController.playerController.aimAngle + 90);
			}
			else
			{
				main.flipRotation = 0;
			}	

			// spellController.playerController.StillTurn();

			// plays particle system
			acidParticleSystem.Play();
		}));
	}

	void CollisionHandler(GameObject collider)
	{
		// checks if enemy
		if (collider.CompareTag("Enemy"))
		{
			// 50% chance of not doing anything, since too many acid drops
			if (Random.Range(0, 2) == 0) return;

			Vector2 adjustedKnockback = knockback;

			if (collider.transform.position.x < spellController.playerController.transform.position.x) adjustedKnockback.x *= -1f;

			collider.GetComponent<EnemyController>().TakeHit(damage, adjustedKnockback);
		}
	}


	void Update()
	{
		// makes acid follow player
		acid.transform.position = spellController.transform.position;
	}

	void OnDestroy()
	{
		OnDisable();
	}

	void OnDisable()
	{
		if(!!acid) acid.GetComponent<DestroyParticlesWhenFinished>().DestroyWhenReady();
	}
}