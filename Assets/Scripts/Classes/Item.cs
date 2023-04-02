using System;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item/Item", order = 1)]
public class Item : ScriptableObject
{
	// stats
	public string name;

	public string description;

	public Sprite icon;

	// subclasses
	[CreateAssetMenu(fileName = "Armor", menuName = "ScriptableObjects/Item/Armor", order = 1)]
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

	[CreateAssetMenu(fileName = "Spell", menuName = "ScriptableObjects/Item/Spell", order = 1)]
	public class Spell : Item
	{
		public Sprite sprite; // this will appear on hand when spell is cast, can be empty

		public GameObject controller; // this is a child game object that can do more complex things with items such as sound effects, popups, and debuffs and is usually a loaded prefab

		public float speed = 1f; // this can be set to 0, making the spell last until the controller sets castingFinished in the playerController

		public Spell(string name, string description, float speed)
		{
			this.name = name;
			this.description = description;
			this.speed = speed;
		}
	}

	[CreateAssetMenu(fileName = "SwingWeapon", menuName = "ScriptableObjects/Item/SwingWeapon", order = 1)]
	public class Weapon : Item
	{
		public Sprite sprite;

		public GameObject controller; // this is a child game object that can do more complex things with items such as sound effects, popups, and debuffs and is usually a loaded prefab

		public float damage;

		public Vector2 knockback;

		public float speed = 1f;

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

		[CreateAssetMenu(fileName = "ChainWeapon", menuName = "ScriptableObjects/Item/ChainWeapon", order = 1)]
		public class ChainWeapon : Weapon
		{
			public Sprite chainSprite;

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
}