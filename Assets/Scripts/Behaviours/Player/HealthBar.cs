using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	[Header("Objects")]
	public Image outline;
	public Image fill;
	public Image fillBackground;
	public Image heart;
	public Animator animator;

	[Header("Constants")]
	public float healthPerWidth = 2;
	public float fillOffset = 10;
	public Color heartColor;
	public Color deathColor;

    public void SetMaxHealth(float health = 10, bool healthAtMax = true)
	{
        // calculates width from offset, health, and health per width
        float width = (health * healthPerWidth) + fillOffset;

        // adjusts with of outline and fillbackground
        outline.GetComponent<RectTransform>().sizeDelta = new Vector2(width, outline.GetComponent<RectTransform>().sizeDelta.y);
        fillBackground.GetComponent<RectTransform>().sizeDelta = new Vector2(width, fillBackground.GetComponent<RectTransform>().sizeDelta.y);
        
        // if health is supposed to show at max, fill is adjusted
        if(healthAtMax) SetHealth(health, false);
    }

    public void SetHealth(float health, bool animate = true)
    {
        // calculates width from offset, health, and health per width
        float width = (health * healthPerWidth) + fillOffset;

        // caps width at the width of the outline
        width = Mathf.Min(width, outline.GetComponent<RectTransform>().sizeDelta.x);

        // adjusts with of outline and fillbackground
        fill.GetComponent<RectTransform>().sizeDelta = new Vector2(width, fill.GetComponent<RectTransform>().sizeDelta.y);

        // plays animation for health change
        if(animate) animator.SetTrigger("healthBarChange");

        // checks if health is zero
        if(health <= 0)
		{
            // sets heart color to gray to indicate death
            heart.color = deathColor;
		}
        else
		{
            // heart is red by default
            heart.color = heartColor;
		}
    }
}
