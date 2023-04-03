using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class ConversationPopup : MonoBehaviour
{
    public GameObject content;
    public Conversation conversation;
	public DialogueManager dialogueManager;
	public bool triggered = false;

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.CompareTag("Player"))
		{
			content.SetActive(true);
			triggered = false;
		}
	}

	void OnTriggerStay2D(Collider2D collider)
	{
		if (!triggered && Input.GetButton("Talk"))
		{
			triggered = true;
			dialogueManager.gameObject.SetActive(true);
			dialogueManager.StartConversation(conversation);
		}
	}

	void OnTriggerExit2D(Collider2D collider)
	{
		if (collider.CompareTag("Player"))
		{
			content.SetActive(false);
			triggered = false;
		}
	}
}
