public class Class
{
	/// <summary>
	/// Equipmnent included with the class
	/// </summary>
	public Item[] equipment;

	public virtual void LevelUp(PlayerData playerData, bool levelIncrease) {}

	public class Warrior : Class
	{
		public override void LevelUp(PlayerData playerData, bool levelIncrease = false)
		{
			playerData.characterData.level++;

			switch (playerData.characterData.level)
			{
				case 1:

					// increased strength for warrior
					playerData.characterData.abilities.strength.modifier++;

					break;

				case 2:
					break;

				case 3:
					break;

				case 4:
					break;

				case 5:
					break;
			}
		}	
	}

	public class Mage : Class
	{
		public override void LevelUp(PlayerData playerData, bool levelIncrease = false)
		{
			playerData.characterData.level++;

			switch (playerData.characterData.level)
			{
				case 1:

					// increased intelligence for mage
					playerData.characterData.abilities.intelligence.modifier++;

					break;

				case 2:
					break;

				case 3:
					break;

				case 4:
					break;

				case 5:
					break;
			}
		}
	}

	public class Rogue : Class
	{
		public override void LevelUp(PlayerData playerData, bool levelIncrease = false)
		{
			playerData.characterData.level++;

			switch (playerData.characterData.level)
			{
				case 1:

					// increased dexterity for rogue
					playerData.characterData.abilities.dexterity.modifier++;

					break;

				case 2:
					break;

				case 3:
					break;

				case 4:
					break;

				case 5:
					break;
			}
		}
	}
}
