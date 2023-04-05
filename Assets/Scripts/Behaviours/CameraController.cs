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

		// stay within boundaries
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

		// if both bounds of a given dimension are visible, stay between them
		if(xMax != null && xMin != null && targetPosition.x + cameraHalfWidth >= xMax.position.x && targetPosition.x - cameraHalfWidth <= xMin.position.x)
		{
			targetPosition.x = (xMin.position.x + xMax.position.x) / 2f;
		}

		if (yMax != null && yMin != null && targetPosition.y + cameraHalfWidth >= yMax.position.y && targetPosition.y - cameraHalfWidth <= yMin.position.y)
		{
			targetPosition.y = (yMin.position.y + yMax.position.y) / 2f;
		}

		transform.position = targetPosition;
	}
}
