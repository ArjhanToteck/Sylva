using UnityEngine;

public class SetActive : MonoBehaviour
{
	public GameObject target;

	public void DisableObject()
	{
		target.SetActive(false);
	}

	public void EnableObject()
	{
		target.SetActive(true);
	}
}
