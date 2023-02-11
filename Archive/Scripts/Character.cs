using System.Collections.Generic;

public class Character
{
	public int level;
	public int speed;

	// damage resistances
	public Damage.DamageResistances damageResistances;

	// hit points
	public HitPoints hitPoints;

	// abilities
	public Ability.AbilitySet abilities;

	// methods
	public void TakeDamage(Damage[] damagesInput = null)
	{
		Damage[] damages = damagesInput ?? new Damage[]{ new Damage() };

		foreach(Damage damage in damages)
		{
			// immunity
			if (!damage.ignoreImmunity && new List<Damage.Types>(damageResistances.immunities).Contains(damage.type))
			{
				continue;
			}

			// resistance
			if(!damage.ignoreResistance && new List<Damage.Types>(damageResistances.resistances).Contains(damage.type))
			{
				DecreaseHitPoints(damage.amount / 2);
				continue;
			}

			// vulnerability
			if (!damage.ignoreVulnerability && new List<Damage.Types>(damageResistances.vulnerabilities).Contains(damage.type))
			{
				DecreaseHitPoints(damage.amount * 2);
			}
		}
	}

	public void DecreaseHitPoints(int amount)
	{
		// checks if any temporary hp
		if(hitPoints.temporary > 0)
		{
			// decreases temporary hp
			hitPoints.temporary -= amount;

			// checks if temporary hp is now negative
			if(hitPoints.temporary < 0) {
				// decreases regular current hitpoints by leftover from temporary
				hitPoints.current += hitPoints.temporary;

				// floors temporary hitpoints to 0
				hitPoints.temporary = 0;
			}
		}
		else
		{
			// reduces hitpoints by amount if no temporary hp
			hitPoints.current -= amount;
		}

		// checks if 0 or less hp
		if(hitPoints.current <= 0)
		{
			// TODO: add death code here later
		}
	}

	// classes
	public class HitPoints
	{
		public int current;
		public int max;
		public int temporary;

		public HitPoints(int currentInput)
		{
			current = currentInput;

			// max is the same as current by default
			max = current;

			// no temporary hit points by default
			temporary = 0;
		}

		public HitPoints(int currentInput, int maxInput, int temporaryInput = 0)
		{
			current = currentInput;
			max = maxInput;
			temporary = temporaryInput;
		}
	}

	public class Ability
	{
		public enum Types
		{
			Charisma,
			Dexterity,
			Intelligence,
			Strength
		}

		/// <summary>
		/// The type of ability (can be charisma, dexterity, intelligence, or strength)
		/// </summary>
		public Types type;

		/// <summary>
		/// The ability modifier, which is added to rolls made with this ability
		/// </summary>
		public int modifier;

		public class AbilitySet
		{
			public Ability charisma;
			public Ability dexterity;
			public Ability intelligence;
			public Ability strength;

			/// <summary>
			/// A set of all four abilities. Each ability defaults to 10.
			/// </summary>
			public AbilitySet(int charismaInput = 0, int dexterityInput = 0, int intelligenceInput = 0, int strengthInput = 0)
			{
				charisma = new Ability(charismaInput, Types.Charisma);
				dexterity = new Ability(dexterityInput, Types.Dexterity);
				intelligence = new Ability(intelligenceInput, Types.Intelligence);
				strength = new Ability(strengthInput, Types.Strength);
			}
		}

		/// <summary>
		/// An object representing an ability with a type, score, and modifier
		/// </summary>
		/// <param name="typeInput">The type of ability (can be charisma, dexterity, intelligence, or strength)</param>
		/// <param name="modifierInput">The ability modifier, which is added to rolls made with this ability</param>
		public Ability(int modifierInput, Types typeInput)
		{
			modifier = modifierInput;
			type = typeInput;
		}

	}

	public Character(int levelInput = 1, int speedInput = 30, Damage.DamageResistances damageResistancesInput = null, HitPoints hitPointsInput = null, Ability.AbilitySet abilitiesInput = null)
	{
		level = levelInput;
		speed = speedInput;

		damageResistances = damageResistancesInput ?? new Damage.DamageResistances();
		hitPoints = hitPointsInput ?? new HitPoints(10);
		abilities = abilitiesInput ?? new Ability.AbilitySet();
	}
}