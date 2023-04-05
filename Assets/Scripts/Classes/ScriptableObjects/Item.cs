using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/Item/Item", order = 1)]
public class Item : ScriptableObject
{
	public string name;
	public string description;
	public Sprite icon;
}