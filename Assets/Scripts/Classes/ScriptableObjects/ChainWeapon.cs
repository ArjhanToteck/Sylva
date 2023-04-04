using UnityEngine;

[CreateAssetMenu(fileName = "ChainWeapon", menuName = "Item/ChainWeapon", order = 1)]
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
	}
}