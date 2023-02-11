using System;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

[Serializable]
public class Item
{
	// stats
	[NonSerialized]
	public string name;

	[NonSerialized]
	public string description;

	[Editable]
	public Sprite icon;

	// subclasses
	[Serializable]
	public class Armor : Item
	{
		public enum ArmorPart
		{
			Helmet,
			Shirt,
			Pants
		}

		[Editable]
		public Texture2D spriteSheet;

		[Editable]
		public Texture2D sleevesSpritesheet; // sleeves are only used in shirts

		[Editable]
		public GameObject controller; // this is a child game object that can do more complex things with items such as sound effects, popups, and debuffs and is usually a loaded prefab

		[NonSerialized]
		public ArmorPart armorPart;

		[NonSerialized]
		public Color color; // only really used for default clothes

		[NonSerialized]
		public float defense; // defense that armor adds

		[NonSerialized]
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

	[Serializable]
	public class Spell : Item
	{
		[Editable]
		public Sprite sprite; // this will appear on hand when spell is cast, can be empty

		[Editable]
		public GameObject controller; // this is a child game object that can do more complex things with items such as sound effects, popups, and debuffs and is usually a loaded prefab

		[NonSerialized]
		public float speed = 1f; // this can be set to 0, making the spell last until the controller sets castingFinished in the playerController

		public Spell(string name, string description, float speed)
		{
			this.name = name;
			this.description = description;
			this.speed = speed;
		}
	}

	[Serializable]
	public class Weapon : Item
	{
		[Editable]
		public Sprite sprite;

		[Editable]
		public GameObject controller; // this is a child game object that can do more complex things with items such as sound effects, popups, and debuffs and is usually a loaded prefab

		[NonSerialized]
		public float damage;

		[NonSerialized]
		public Vector2 knockback;

		[NonSerialized]
		public float speed = 1f;

		[NonSerialized]
		public Action<GameObject, Collider2D> OnHit;

		// constructor
		public Weapon(string name, string description, int damage, float speed, Vector2 knockback)
		{
			this.name = name;
			this.description = description;
			this.damage = damage;
			this.speed = speed;
			this.knockback = knockback;

			OnHit = (GameObject gameObject, Collider2D enemy) =>
			{
				Vector2 adjustedKnockback = knockback;

				if (enemy.transform.position.x < gameObject.transform.position.x) adjustedKnockback.x *= -1f;

				enemy.GetComponent<EnemyController>().TakeHit(damage, adjustedKnockback);
			};
		}

		[Serializable]
		public class ChainWeapon : Weapon
		{
			[Editable]
			public Sprite chainSprite;

			[NonSerialized]
			public float range;

			public ChainWeapon(string name, string description, int damage, float speed, Vector2 knockback, float range) : base(name, description, damage, speed, knockback)
			{
				this.name = name;
				this.description = description;
				this.damage = damage;
				this.speed = speed;
				this.knockback = knockback;
				this.range = range;

				OnHit = (GameObject gameObject, Collider2D enemy) =>
				{
					Vector2 adjustedKnockback = knockback;

					if (enemy.transform.position.x < gameObject.transform.position.x) adjustedKnockback.x *= -1f;

					enemy.GetComponent<EnemyController>().TakeHit(damage, adjustedKnockback);
				};
			}
		}
	}

	// contains list of all items
	public static Items items = new Items();

	[Serializable]
	public class Items
	{
		// armor

		// default set

		public Armor defaultShirt = new Armor
		(
			"Default Shirt",
			"This shirt was canonically stolen from a homeless man.",
			Armor.ArmorPart.Shirt,
			color: new Color(0.3f, 0.3f, 0.3f)
		);

		public Armor defaultPants = new Armor
		(
			"Default Pants",
			"Some random pants you probably found in a dumpster.",
			Armor.ArmorPart.Pants,
			color: new Color(0.1f, 0.2f, 0.5f)
		);

		// gentleman set

		public Armor topHat = new Armor
		(
			"Top Hat",
			"The helmet of a true gentleman.",
			Armor.ArmorPart.Helmet,
			defense: 1,
			color: new Color(0.2f, 0.2f, 0.2f)
		);

		// TODO: add dress pants + suit and tie (gentleman set)

		// spells

		public Spell acidSpray = new Spell(
			"Acid Spray",
			"Make acid leap from your fingers to your enemies.",
			1f
		);

		public Spell bouncingFlame = new Spell(
			"Bouncing Flame",
			"Creates a ball that bounces around and hits enemies. Pretty self explanatory.",
			0.9f
		);

		public Spell tempest = new Spell(
			"Tempest",
			"Summons lightning to strike down enemies around you.",
			1.25f
		);

		// weapons

		// swing weapons

		public Weapon apprenticeShortSword = new Weapon
		(
			"Apprentice Shortsword",
			"Just a basic sword for apprentices. Get a real weapon, loser.",
			 4,
			 0.8f,
			 new Vector2(10f, 8f)
		);

		public Weapon comicallyLargeSpoon = new Weapon
		(
			"Comically Large Spoon",
			"Hey, dawg, can I get some ice cream?",
			 5,
			 0.9f,
			 new Vector2(15f, 8f)
		);

		public Weapon electricGuitar = new Weapon
		(
			"Electric Guitar",
			"Confuse your enemies by smacking them with an actual damn guitar.",
			 5,
			 0.5f,
			 new Vector2(10f, 8f)
		);

		public Weapon firstOfArdor = new Weapon
		(
			"Fist of Ardor",
			"This blade of pure silver burns with the power of Ardor, god of justice. Striking an enemy lights them on fire.",
			 4,
			 0.5f,
			 new Vector2(15f, 8f)
		);

		public Weapon ironMace = new Weapon
		(
			"Iron Mace",
			"A heavy iron head attatched to a short wooden shaft, making a crude bludgeoning weapon.",
			 5,
			 0.9f,
			 new Vector2(15f, 8f)
		);

		public Weapon steelWarHammer = new Weapon
		(
			"Steel War Hammer",
			"Because they double as a tool, hammers like these common among the nomadic warriors of the north. They're slow, but strong.",
			 7,
			 0.4f,
			 new Vector2(15f, 8f)
		);

		public Weapon viperKnife = new Weapon
		(
			"Viper Knife",
			"They say the only thing you see is a streak of white before you realize you're bleeding out.",
			 3,
			 1.25f,
			 new Vector2(5f, 8f)
		);

		// chain weapons

		public Weapon.ChainWeapon flail = new Weapon.ChainWeapon
		(
			"Flail",
			"Spike ball on chain. Great weapon.",
			 3,
			 0.75f,
			 new Vector2(8f, 8f),
			 8f
		);
	}

	// mark a property or field as editable in inspector
	internal class EditableAttribute : Attribute {}
}