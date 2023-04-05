using System;
using UnityEngine;

public class SerpentTongue : MonoBehaviour
{
	ChainWeaponController chainWeaponController;

	public GameObject burnEffect;
	public GameObject fireTrail;

	bool unequipped = false;

	// Start is called before the first frame update (as soon as weapon is equipped)
	void Start()
	{
		// loads chain weapon controller
		chainWeaponController = transform.parent.Find("ChainWeapon").GetComponent<ChainWeaponController>();

		// clones fire trail so it isn't deleted when weapon is unequipped
		fireTrail = Instantiate(fireTrail);

		// adds event to be played when weapon hit enemy
		chainWeaponController.onHitEnemyEvents.Add(new Action<Collider2D>((Collider2D collider) =>
		{
			// checks if not already burning
			if (!collider.transform.Find("BurnEffect"))
			{
				// adds burn effect to enemy
				GameObject burnEffectClone = Instantiate(burnEffect);
				burnEffectClone.GetComponent<BurnEffect>().victim = collider.gameObject;
			}
		}));

		// starts fire trail on attack
		chainWeaponController.onAttackEvents.Add(new Action(() =>
		{
			fireTrail.GetComponent<ParticleSystem>().Play();
		}));

		// stops fire trail on attack finish
		chainWeaponController.onAttackFinishEvents.Add(new Action(() =>
		{
			fireTrail.GetComponent<ParticleSystem>().Stop();
		}));
	}

	void Update()
	{
		if (unequipped) return;

		// makes fire trail follow weapon

		// makes sure weapon is enabled
		if (chainWeaponController.gameObject.activeInHierarchy)
		{
			// copies position and rotation
			fireTrail.transform.position = (Vector2)chainWeaponController.head.transform.position;
			fireTrail.transform.rotation = chainWeaponController.head.transform.rotation;

			/*// accounts for direction of weapon
			if (chainWeaponController.playerController.transform.localScale.x < 0)
			{
				fireTrail.transform.localScale = new Vector3(-Mathf.Abs(fireTrail.transform.localScale.x), fireTrail.transform.localScale.y, fireTrail.transform.localScale.z);
			}
			else
			{
				fireTrail.transform.localScale = new Vector3(Mathf.Abs(fireTrail.transform.localScale.x), fireTrail.transform.localScale.y, fireTrail.transform.localScale.z);
			}*/
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
