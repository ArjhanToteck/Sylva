using UnityEngine;

public class GuitarController : MonoBehaviour
{
    public AudioSource audioManager;
    public AudioClip guitarSmash;

    SwingWeaponController SwingWeaponController;

    // Start is called before the first frame update (as soon as weapon is equipped)
    void Start()
    {
        // loads audio source
        audioManager = FindObjectOfType<AudioSource>();

        // loads swing weapon controller
        SwingWeaponController = transform.parent.Find("SwingWeapon").GetComponent<SwingWeaponController>();

        // adds event to be played when weapon hit something
        SwingWeaponController.onHitEvents.Add(new System.Action<Collider2D>((Collider2D collider) =>
        {
            // plays guitar smash sound effect
            audioManager.PlayOneShot(guitarSmash);
        }));
    }
}
