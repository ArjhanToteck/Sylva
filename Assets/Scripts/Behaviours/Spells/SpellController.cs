using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellController : MonoBehaviour
{
	public Item.Spell spell;
	public PlayerController playerController;

	// events for controller children
	public List<Action> onCastEvents = new List<Action>();
	public List<Action> onCastFinishEvents = new List<Action>();

	void OnDisable()
	{
		// checks if casting is finished
		if (playerController.castSpellProgress > 0.99f)
		{
			// resets progress to -1 to show that its finished
			playerController.castSpellProgress = -1f;

			// calls all events for onCastFinish
			foreach (Action action in onCastFinishEvents)
			{
				action();
			}
		}
		else
		{
			// this makes it so even if a player starts running while casting a spell, the spell does not go away
			playerController.castSpell = true;
		}

		//playerController.inSwingAttackAnimation = false;
	}

	public void TriggerOnCastEvents()
	{
		foreach (Action action in onCastEvents)
		{
			action();
		}
	}

	public void ClearEvents()
	{
		onCastEvents.Clear();
		onCastFinishEvents.Clear();
	}
}
