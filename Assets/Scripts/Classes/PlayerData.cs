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

	[NonSerialized]
	public Color hairColor = new Color(0.2f, 0f, 0f);

	[NonSerialized]
	public Color skinColor = new Color(0.8f, 0.6f, 0.5f);

	// inventory

	[NonSerialized]
	public List<Item.Weapon> weapons = new List<Item.Weapon>(new Item.Weapon[]{
		Item.items.apprenticeShortSword,
		Item.items.fistOfArdor,
		Item.items.steelWarHammer,
		Item.items.flail,
		Item.items.viperKnife,
		Item.items.electricGuitar,
		Item.items.comicallyLargeSpoon
	});

	[NonSerialized]
	public List<Item.Spell> spells = new List<Item.Spell>(new Item.Spell[]{
		Item.items.acidSpray,
		Item.items.tempest
	});

	[NonSerialized]
	public ArmorSet armor = new ArmorSet(
		Item.items.topHat,
		Item.items.defaultShirt,
		Item.items.defaultPants
	);

	[Serializable]
	public class ArmorSet
	{
		public Item.Armor helmet;
		public Item.Armor shirt;
		public Item.Armor pants;

		public ArmorSet(Item.Armor helmet, Item.Armor shirt, Item.Armor pants)
		{
			this.helmet = helmet;
			this.shirt = shirt;
			this.pants = pants;
		}
	}
}
