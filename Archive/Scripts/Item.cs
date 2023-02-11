using System;

public class Item
{
	// local variables
	public string name;
	public Types type;
	public int value;

	// item types
	public enum Types
	{
		Armor,
		Consumable,
		Food,
		Material,
		Miscellaneous,
		QuestItem,
		Weapon
	}
}