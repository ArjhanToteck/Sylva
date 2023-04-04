using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{	public UltEvent onTrigger;

	bool triggered = false;

	void OnTriggerEnter2D(Collider2D collision)
	{
		if (triggered) return;

		if (collision.CompareTag("Player"))
		{
			triggered = true;
			onTrigger.Invoke();
			Destroy(gameObject);
		}
	}
}
