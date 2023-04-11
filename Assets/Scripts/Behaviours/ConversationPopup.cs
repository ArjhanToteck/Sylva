using UltEvents;
using UnityEngine;

public class ConversationPopup : MonoBehaviour
{
    public GameObject label;
	public UltEvent action;

	public bool triggered = false;

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.CompareTag("Player"))
		{
			label.SetActive(true);
			triggered = false;
		}
	}

	void OnTriggerStay2D(Collider2D collider)
	{
		if (!triggered && Input.GetButton("Interact"))
		{
			triggered = true;
			action.Invoke();
		}
	}

	void OnTriggerExit2D(Collider2D collider)
	{
		if (collider.CompareTag("Player"))
		{
			label.SetActive(false);
			triggered = false;
		}
	}
}
