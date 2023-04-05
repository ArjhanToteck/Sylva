using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 offset;

	public Camera camera;
	public Transform player;

    public Transform xMin;
	public Transform xMax;
	public Transform yMin;
	public Transform yMax;

	
	void Update()
    {
		// get half the camera size
		float cameraHalfHeight = camera.orthographicSize;
		float cameraHalfWidth = cameraHalfHeight * camera.aspect;

		// follows player
		Vector3 targetPosition = player.position + offset;

        if(xMin != null)
        {
            targetPosition.x = Mathf.Max(targetPosition.x, xMin.position.x + cameraHalfWidth);
		}

		if(yMin != null)
		{
			targetPosition.y = Mathf.Max(targetPosition.y, yMin.position.y + cameraHalfHeight);
		}

		if (xMax != null)
		{
			targetPosition.x = Mathf.Min(targetPosition.x, xMax.position.x - cameraHalfWidth);
		}

		if (yMax != null)
		{
			targetPosition.y = Mathf.Min(targetPosition.y, yMax.position.y - cameraHalfHeight);
		}

		transform.position = targetPosition;
	}
}
