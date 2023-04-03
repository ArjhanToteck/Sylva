using System;
using UnityEngine;

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
}