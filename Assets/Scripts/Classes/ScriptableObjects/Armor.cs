using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Armor", menuName = "ScriptableObject/Item/Armor", order = 1)]
public class Armor : Item
{
	public enum ArmorPart
	{
		Helmet,
		Shirt,
		Pants
	}

	public Texture2D spriteSheet;

	public Texture2D sleevesSpritesheet; // sleeves are only used in shirts

	public GameObject controller; // this is a child game object that can do more complex things with items such as sound effects, popups, and debuffs and is usually a loaded prefab 

	public ArmorPart armorPart;

	public Color color; // only really used for default clothes

	public float defense; // defense that armor adds

	public bool hideHair; // whether or not to hide hair, only used in headwear

	// constructor
	public Armor(string name, string description, ArmorPart bodyPart, float defense = 0, bool hideHair = false, object color = null)
	{
		this.name = name;
		this.description = description;
		this.armorPart = bodyPart;
		this.defense = defense;
		this.hideHair = hideHair;
		this.color = color == null ? Color.white : (Color)color;
	}
}
