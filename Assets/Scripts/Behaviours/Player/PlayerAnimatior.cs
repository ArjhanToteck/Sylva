using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatior : MonoBehaviour
{
	[Header("Settings")]
	public Texture2D spriteSheet;
	public Color defaultColor;

	const int spriteWidth = 48;
	const int spriteHeight = 48;
	const int pixelsPerUnit = 8;

	[Header("Sprites")]
	SpriteRenderer spriteRenderer;
	List<Sprite> sprites = new List<Sprite>();

	void Start()
	{
		UpdateClothes();
	}

	public void UpdateClothes()
	{
		ResetSprites();
		GetDefaultColor();
	}

	void GetDefaultColor()
	{
		defaultColor = spriteRenderer.color;
	}

	void ResetSprites()
	{
		// gets sprite renderer
		spriteRenderer = transform.parent.GetComponent<SpriteRenderer>();

		// loads sprites from spritesheet
		for (int x = spriteSheet.height; x > 0; x -= spriteHeight)
		{
			for (int y = 0; y < spriteSheet.width; y += spriteWidth)
			{
				// slices sprite from sheet
				Sprite sprite = Sprite.Create(spriteSheet, new Rect(y, x - spriteHeight, spriteWidth, spriteHeight), new Vector2(0.5f, 0.5f), pixelsPerUnit);
				sprite.name = spriteSheet.name + "_" + sprites.Count;

				// adds sprite to list
				sprites.Add(sprite);
			}
		}
	}

	public virtual void SetSprite(int frame)
	{
		// sets sprite to current frame in sheet
		spriteRenderer.sprite = sprites[frame];
	}

	public void SetLightness(float amount)
	{
		// sets sprite color
		spriteRenderer.color = Color.Lerp(defaultColor, Color.black, amount);
	}
}
