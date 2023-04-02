using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
	public int maxHealth = 10;
	public int health = 10;
	public float hitCooldown = 3f;
	public float speed = 60f;
	public float defense = 0f;
	public int selectedWeapon = 1;
	public int selectedSpell = 0;

	// hair isn't an item like armor, so must be tracked in PlayerData
	public Texture2D hair;

	public Color hairColor = new Color(0.2f, 0f, 0f);

	public Color skinColor = new Color(0.8f, 0.6f, 0.5f);

	// inventory

	public List<Item.Weapon> weapons = new List<Item.Weapon>();

	public List<Item.Spell> spells = new List<Item.Spell>();

	public ArmorSet armor = new ArmorSet();

	[Serializable]
	public class ArmorSet
	{
		public Item.Armor helmet;
		public Item.Armor shirt;
		public Item.Armor pants;

		public ArmorSet()
		{
			helmet = null;
			shirt = null;
			pants = null;
		}

		public ArmorSet(Item.Armor helmet, Item.Armor shirt, Item.Armor pants)
		{
			this.helmet = helmet;
			this.shirt = shirt;
			this.pants = pants;
		}
	}
}
