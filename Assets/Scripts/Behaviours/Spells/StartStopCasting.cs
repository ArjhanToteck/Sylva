using UnityEngine;

public class StartStopCasting : MonoBehaviour
{
	public PlayerController playerController;
	public SpellController spellController;

	public void StartCasting()
	{
		playerController.castSpellProgress = 0;
		spellController.TriggerOnCastEvents();
	}

	public void StopCasting()
	{
		playerController.castSpellProgress = -1;
	}

}
