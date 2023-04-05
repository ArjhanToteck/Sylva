using UnityEngine;

public class ArmsAnimator : PlayerAnimatior
{
	public PlayerController controller;
	public Animator animator;

    public override void SetSprite(int frame)
	{
		if ((animator.GetCurrentAnimatorStateInfo(1).IsName("ArmsSwingAttackRun") || animator.GetCurrentAnimatorStateInfo(1).IsName("ArmsChainAttackRun") || animator.GetCurrentAnimatorStateInfo(1).IsName("ArmsCastSpellRun")) && frame < 25) return;

		base.SetSprite(frame);
	}
}