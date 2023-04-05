using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObject/Quest", order = 1)]
[Serializable]
public class Quest : ScriptableObject
{
	static List<Quest> quests = new List<Quest>();
	public static QuestManager questManager;

	public string name;
	public string description;
	public QuestType type;
	public Status status = Status.NotStarted;
	public Progress progress;

	[NonSerialized]
	int id;

	[Serializable]
	public enum QuestType
	{
		Main,
		Side
	}

	[Serializable]
	public enum Status
	{
		NotStarted,
		Started,
		Complete,
		Failed
	}

	[Serializable]
	public class Progress
	{
		public int totalSteps = -1;
		public int stepsCompleted = -1;
	}

	public void Awake()
	{
		quests.Add(this);
		id = quests.Count;
	}

	public void IncrementProgress()
	{
		if (progress.totalSteps == -1) return;

		progress.stepsCompleted++;

		if(progress.stepsCompleted >= progress.totalSteps)
		{
			CompleteQuest();
		}
	}

	public void StartQuest()
	{
		status = Status.Started;
		questManager.StartQuest(this);
	}

	public void UpdateQuest(string newDescription)
	{
		description = newDescription;
		questManager.UpdateQuest(this);
	}

	public void CompleteQuest()
	{
		status = Status.Complete;
		questManager.CompleteQuest(this);
	}
}
