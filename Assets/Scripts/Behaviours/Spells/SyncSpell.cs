using UnityEngine;

public class SyncSpell : MonoBehaviour
{
    public PlayerController playerController;
	public Animator animator;

	public bool spell = false;

	void Update()
	{
		if (!spell) return;

		// updates spell progress

		// loops through layers in animator
		for (int i = 0; i < animator.layerCount; i++)
		{
			// checks if attacking using current layer
			if (animator.GetCurrentAnimatorStateInfo(i).IsTag("CastSpell"))
			{
				playerController.castSpellProgress = animator.GetCurrentAnimatorStateInfo(i).normalizedTime;

				return;
			}
		}
	}

	public void Sync()
    {
		if (playerController.castSpellProgress <= 0) return;

		// updates attack progress

		// loops through layers in animator
		for (int i = 0; i < animator.layerCount; i++)
		{
			// checks if attacking using current layer
			if (animator.GetCurrentAnimatorStateInfo(i).IsTag("CastSpell"))
			{
				int clip = animator.GetCurrentAnimatorStateInfo(i).fullPathHash;
				animator.Play(clip, i, playerController.castSpellProgress);

				return;
			}
		}
	}
}