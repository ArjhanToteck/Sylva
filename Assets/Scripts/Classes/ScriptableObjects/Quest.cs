using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Quest", menuName = "Quest", order = 1)]
public class Quest : ScriptableObject
{
	public static List<Quest> quests = new List<Quest>();

	public static void StartQuest(Quest quest)
	{
		// TODO: implement this
	}

	public QuestType questType;
	public string name;
	public string description;

	public Quest()
	{
		quests.Add(this);
	}

	public enum QuestType
	{
		Main,
		Side
	}
}
