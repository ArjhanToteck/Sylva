using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Conversation", menuName = "ScriptableObjects/Conversation", order = 1)]
public class Conversation : ScriptableObject
{
	public List<Dialogue> conversation;
}
