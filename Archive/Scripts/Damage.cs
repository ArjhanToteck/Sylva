public class Damage
{
	public enum Types
	{
		Cold,
		Fire,
		Lightning,
		Necrotic,
		Physical,
		Poison,
		Psychic,
		Radiant
	}

	public Types type;
	public int amount;

	public bool ignoreResistance;
	public bool ignoreImmunity;
	public bool ignoreVulnerability;

	/// <summary>Stores a type and amount of damage.</summary>
	/// <param name="typeInput">Can be Cold, Fire, Lightning, Necrotic, Physical, Poison, Psychic, Radiant. Physical by default.</param>
	/// <param name="amountInput">The amount of damage. 1 by default.</param>
	/// <param name="ignoreResistanceInput">Will ignore resistance if true. False by default.</param>
	/// <param name="ignoreImmunityInput">Will ignore immunity if true. False by default.</param>
	public Damage(Types typeInput = Types.Physical, int amountInput = 1, bool ignoreResistanceInput = false, bool ignoreImmunityInput = false, bool ignoreVulnerabilityInput = false)
	{
		type = typeInput;
		amount = amountInput;
		ignoreResistance = ignoreResistanceInput;
		ignoreImmunity = ignoreImmunityInput;
		ignoreVulnerability = ignoreVulnerabilityInput;
	}

	public class DamageResistances
	{
		public Types[] immunities;
		public Types[] resistances;
		public Types[] vulnerabilities;

		public DamageResistances(Types[] immunitiesInput = null, Types[] resistancesInput = null, Types[] vulnerabilitiesInput = null)
		{
			immunities = immunitiesInput ?? new Types[0];
			resistances = resistancesInput ?? new Types[0];
			vulnerabilities = vulnerabilitiesInput ?? new Types[0];
		}
	}
}