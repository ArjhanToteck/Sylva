using System;
using UnityEngine;

[Serializable]
public class EnemyData
{
	public string name = "Slime";
	public int health = 15;
	public float speed = 45f;
	public EnemyController controller;

	public int damage = 3;
	public Vector2 knockback = new Vector2(20f, 15f);
}