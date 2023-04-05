using UnityEngine;

public class SyncAttack : MonoBehaviour
{
    public PlayerController playerController;
	public SwingWeaponController swingWeaponController;
	public Animator animator;

	public bool weapon = false;

	void Update()
	{
		if (!weapon) return;

		// updates attack progress

		// loops through layers in animator
		for (int i = 0; i < animator.layerCount; i++)
		{
			// checks if attacking using current layer
			if (animator.GetCurrentAnimatorStateInfo(i).IsTag("SwingAttack"))
			{
				playerController.swingAttackProgress = animator.GetCurrentAnimatorStateInfo(i).normalizedTime;

				return;
			}
		}
	}

	public void Sync()
    {
		if (playerController.swingAttackProgress <= 0) return;

		// updates attack progress

		// loops through layers in animator
		for (int i = 0; i < animator.layerCount; i++)
		{
			// checks if attacking using current layer
			if (animator.GetCurrentAnimatorStateInfo(i).IsTag("SwingAttack"))
			{
				int clip = animator.GetCurrentAnimatorStateInfo(i).fullPathHash;
				animator.Play(clip, i, playerController.swingAttackProgress);

				return;
			}
		}
	}

	public void TriggerOnSwingEvents()
	{
		swingWeaponController.TriggerOnSwingEvents();
	}
}