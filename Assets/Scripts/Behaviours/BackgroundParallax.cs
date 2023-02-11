using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    public float length;
    public float startPosition;

    public float parallaxIntensity;

	public GameObject camera;

    void Start()
    {
        // gets settings at start
        startPosition = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    
    void FixedUpdate()
	{
        // gets position relative to camera
        float cameraOffset = camera.transform.position.x * (1 - parallaxIntensity);

        // calculates distance
        float distance = camera.transform.position.x * parallaxIntensity;

        // moves background
        transform.position = new Vector3(startPosition + distance, transform.position.y, transform.position.z);

        // scrolling

        // scrolls right
        if (cameraOffset > startPosition + length)
        {
			startPosition += length;
		}
        // scrolls left
        else if (cameraOffset < startPosition - length)
        {
            startPosition -= length;
        }

    }
}
