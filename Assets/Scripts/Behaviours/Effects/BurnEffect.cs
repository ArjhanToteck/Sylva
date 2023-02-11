using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnEffect : MonoBehaviour
{
	public int damage = 1; // damage dealt every [interval] for [duration]
	public float interval = 1f; // repeating interval in seconds for damage to be dealt
	public float duration = 5; // time in seconds for effect to last

	public List<Action> onFinish = new List<Action>();

	public GameObject victim;

	// Start is called before the first frame update
	void Start()
    {
		// starts hurting victim
		StartCoroutine(DealDamage());
    }

	IEnumerator DealDamage()
	{
		float timeWaited = 0;

		while(timeWaited < duration)
		{
			// checks if victim no longer exists (most likely died)
			if (!victim)
			{
				DestroyWhenReady();
				yield break;
			}

			// deals damage to victim
			victim.GetComponent<EnemyController>().TakeHit(damage, Vector2.zero, false);

			// waits for [interval] seconds
			yield return new WaitForSeconds(interval);
			timeWaited += interval;
		}

		// calls onFinish events
		foreach(Action action in onFinish)
		{
			action();
		}

		DestroyWhenReady();
	}

	void Update()
	{
		// makes burn effect follow victim

		// makes sure victim still exists
		if (!!victim)
		{
			// copies position and rotation (bottom center of victim's sprite)
			transform.position = new Vector2(victim.transform.position.x, victim.GetComponent<SpriteRenderer>().bounds.min.y);
			transform.rotation = victim.transform.rotation;

			// accounts for direction of weapon
			if (victim.transform.localScale.x < 0)
			{
				transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
			}
			else
			{
				transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
			}
		}
	}

	void DestroyWhenReady()
	{
		// destroys fire effect once no more particles exist
		GetComponent<ParticleSystem>().Stop();
		GetComponent<DestroyParticlesWhenFinished>().DestroyWhenReady();
	}
}
