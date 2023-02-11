using UnityEngine;

public class PlayerData
{
	// armor that is worn for protection
	public Item.ArmorSet armorSet;

	// armor worn in vanity slots
	public Item.ArmorSet armorVanitySet;

	// this is default clothes, hair, and body
	public Vanity.VanitySet defaultVanitySet = new Vanity.VanitySet();

	[Header("Stats")]
	public Character characterData = new Character();
	public SkillTree.Skill[] skills;

	[Header("Inventory")]
	public Item[] inventory;
	public int inventorySize;

	public PlayerData()
	{

	}
}