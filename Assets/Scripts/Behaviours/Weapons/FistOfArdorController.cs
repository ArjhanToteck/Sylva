using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistOfArdorController : MonoBehaviour
{
	SwingWeaponController swingWeaponController;

	public GameObject burnEffect;
	public GameObject fireTrail;

	bool unequipped = false;

	// Start is called before the first frame update (as soon as weapon is equipped)
	void Start()
	{
		// loads swing weapon controller
		swingWeaponController = transform.parent.Find("SwingWeapon").GetComponent<SwingWeaponController>();

		// clones fire trail so it isn't deleted when weapon is unequipped
		fireTrail = Instantiate(fireTrail);

		// adds event to be played when weapon hit something
		swingWeaponController.onHitEvents.Add(new Action<Collider2D>((Collider2D collider) =>
		{
			// checks if hit enemy and not already burning
			if (!!collider.GetComponent<EnemyController>() && !collider.transform.Find("BurnEffect"))
			{
				// adds burn effect to enemy
				GameObject burnEffectClone = Instantiate(burnEffect);
				burnEffectClone.GetComponent<BurnEffect>().victim = collider.gameObject;
			}
		}));

		// starts fire trail on swing
		swingWeaponController.onAttackEvents.Add(new Action(() =>
		{
			fireTrail.GetComponent<ParticleSystem>().Play();
		}));

		// stops fire trail on swing finish
		swingWeaponController.onAttackFinishEvents.Add(new Action(() =>
		{
			fireTrail.GetComponent<ParticleSystem>().Stop();
		}));
	}

	void Update()
	{
		if (unequipped) return;

		// makes fire trail follow weapon

		// makes sure weapon is enabled
		if (swingWeaponController.gameObject.activeInHierarchy)
		{
			// copies position and rotation
			fireTrail.transform.position = swingWeaponController.transform.position;
			fireTrail.transform.rotation = swingWeaponController.transform.rotation;

			// accounts for direction of weapon
			if (swingWeaponController.playerController.transform.localScale.x < 0)
			{
				fireTrail.transform.localScale = new Vector3(-Mathf.Abs(fireTrail.transform.localScale.x), fireTrail.transform.localScale.y, fireTrail.transform.localScale.z);
			}
			else
			{
				fireTrail.transform.localScale = new Vector3(Mathf.Abs(fireTrail.transform.localScale.x), fireTrail.transform.localScale.y, fireTrail.transform.localScale.z);
			}
		}
	}

	void OnDestroy()
	{
		// marks as unequipped
		unequipped = true;

		// checks if fire trail still exists
		if (!!fireTrail)
		{
			// stops fire trail
			fireTrail.GetComponent<ParticleSystem>().Stop();

			// destroys fire trail once no more particles exist
			fireTrail.GetComponent<DestroyParticlesWhenFinished>().DestroyWhenReady();
		}

	}
}
