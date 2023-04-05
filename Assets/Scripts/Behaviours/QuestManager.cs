using UnityEngine;

public class QuestManager : MonoBehaviour
{
	void Awake()
	{
		Quest.questManager = this;
	}

	public void StartQuest(Quest quest)
	{

	}

	public void CompleteQuest(Quest quest)
	{

	}
}
