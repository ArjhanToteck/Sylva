using UltEvents;
using UnityEngine;

public class InteractionPopup : MonoBehaviour
{
    public GameObject label;
	public UltEvent action;

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.CompareTag("Player"))
		{
			label.SetActive(true);
		}
	}

	void OnTriggerStay2D(Collider2D collider)
	{
		if (Input.GetButton("Interact"))
		{
			action.Invoke();
		}
	}

	void OnTriggerExit2D(Collider2D collider)
	{
		if (collider.CompareTag("Player"))
		{
			label.SetActive(false);
		}
	}
}
