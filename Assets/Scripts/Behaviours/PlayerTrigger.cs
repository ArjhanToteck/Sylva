using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTrigger : MonoBehaviour
{	public UnityEvent onTrigger;

	bool isTriggered = false;

	void OnTriggerEnter2D(Collider2D collision)
	{
		if (isTriggered) return;

		if (collision.CompareTag("Player"))
		{
			isTriggered = true;
			onTrigger.Invoke();
			Destroy(gameObject);
		}
	}
}
