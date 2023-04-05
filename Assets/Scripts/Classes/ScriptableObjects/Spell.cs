using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "ScriptableObject/Item/Spell", order = 1)]
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