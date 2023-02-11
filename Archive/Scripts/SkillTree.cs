using System;

public static class SkillTree
{
	public static class Skills
	{
		public static Skill fireball = new Skill
		(
			"Fireball",
			"Channel your mana into creating a ball of fire to launch at enemies."
		);

		public static Skill healing = new Skill
		(
			"Healing",
			"Consume mana to replenish a part of your health."
		);

		public static Skill fleche = new Skill
		(
			"Fleche",
			"While dashing, end your movement with a strong thrust of your weapon."
		);

		public static Skill whirlwind = new Skill
		(
			"Whirlwind",
			"Release your rage cutting through enemies in all directions."
		);

		public static Skill dive = new Skill
		(
			"Dive",
			"While in the air, throw yourself down onto your enemies."
		);
	}
    
    public class Skill
	{
		public string name;
		public string description;

		public Action onSkillApplied;

		public Skill[] requirements;


		/// <summary>
		/// A skill that can be unlocked
		/// </summary>
		/// <param name="nameInput">
		/// 
		/// </param>
		/// <param name="requirements">
		/// An array of skills that must be unlocked before this skill. Usually just one.
		/// </param>
		public Skill(string nameInput, string descriptionInput, Action onSkillAppliedInput = null, Skill[] requirementsInput = null)
		{
			name = nameInput;
			description = descriptionInput;
			onSkillApplied = onSkillAppliedInput;
			requirements = requirementsInput;
		}
	}
}
