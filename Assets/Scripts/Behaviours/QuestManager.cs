using UnityEngine;

public class QuestManager : MonoBehaviour
{
	public GameObject alertPrefab;

	void Awake()
	{
		Quest.questManager = this;
	}

	public void StartQuest(Quest quest)
	{
        // TODO: implement alerts for quests
		//GameObject alert = Instantiate(alertPrefab);
	}

	public void UpdateQuest(Quest quest)
	{

		//GameObject alert = Instantiate(alertPrefab);
	}

	public void CompleteQuest(Quest quest)
	{

		//GameObject alert = Instantiate(alertPrefab);
	}
}
