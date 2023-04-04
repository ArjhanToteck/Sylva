using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Quest", menuName = "Quest", order = 1)]
public class Quest : ScriptableObject
{
	public static List<Quest> quests = new List<Quest>();

	public QuestType questType;
	public string name;
	public string description;

	public enum QuestType
	{
		Main,
		Side
	}

	public void StartQuest()
	{
		// TODO: implement this
		quests.Add(this);
		Debug.Log("poop balls");
	}
}
