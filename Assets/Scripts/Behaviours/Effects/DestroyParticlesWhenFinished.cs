using System.Collections;
using UnityEngine;

public class DestroyParticlesWhenFinished : MonoBehaviour
{
	public void DestroyWhenReady()
	{
		StartCoroutine(WaitToDestroy());
	}

	public IEnumerator WaitToDestroy()
	{
		// waits until particles disappear
		while (GetComponent<ParticleSystem>().particleCount > 0)
		{
			yield return null;
		}

		Destroy(gameObject);
	}
}
