using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainWeaponController : MonoBehaviour
{
	public Item.Weapon.ChainWeapon weapon;
	public PlayerController playerController;
	public Transform origin;
	public SpriteRenderer head;
	public SpriteRenderer chain;

	public float angle;
    public float speed;

    public Vector2 targetPosition;

    public bool goingForward = true;

	void OnEnable()
    {
		playerController.chainAttackFinished = false;

		goingForward = true;

        // gets weapon data
		weapon = (Item.Weapon.ChainWeapon)playerController.data.weapons[playerController.data.selectedWeapon];

        // sets proper sprites
        chain.sprite = weapon.chainSprite;
        head.sprite = weapon.sprite;

        // calculates target position
        angle = playerController.aimAngle;
		targetPosition = origin.position;
        targetPosition.x += weapon.range * Mathf.Cos(angle * Mathf.Deg2Rad);
		targetPosition.y += weapon.range * Mathf.Sin(angle * Mathf.Deg2Rad);

		// turns around if attacking backwards
		if (targetPosition.x < playerController.transform.position.x && playerController.transform.localScale.x > 0)
		{
			playerController.Flip();
		}
		else if (targetPosition.x > playerController.transform.position.x && playerController.transform.localScale.x < 0)
		{
			playerController.Flip();
		}

		// positions head at origin
		head.transform.position = origin.position;
	}

    void Update()
    {
		// chain

		// recalculates target position
		angle = playerController.aimAngle;
		targetPosition = origin.position;
		targetPosition.x += weapon.range * Mathf.Cos(angle * Mathf.Deg2Rad);
		targetPosition.y += weapon.range * Mathf.Sin(angle * Mathf.Deg2Rad);

		// calculates midpoint, distance, and direction
		Vector3 midpoint = (origin.position + head.transform.position) / 2;
        float distance = Vector2.Distance(origin.position, head.transform.position);
        Vector2 direction = Vector3.Normalize(head.transform.position - origin.position);

		// sets position, direction, and size
		chain.transform.position = midpoint;
        chain.transform.right = direction;
        chain.drawMode = SpriteDrawMode.Tiled;
		chain.size = new Vector2(distance, chain.size.y);

		// head

		// checks direction
		if (goingForward)
		{
			head.transform.position = Vector2.MoveTowards(head.transform.position, targetPosition, weapon.speed * Time.deltaTime * 25f);

			if (Vector2.Distance(head.transform.position, targetPosition) < 0.1f || Vector2.Distance(head.transform.position, origin.position) >= weapon.range) goingForward = false;
		}
		else
		{
			head.transform.position = Vector2.MoveTowards(head.transform.position, origin.position, weapon.speed * Time.deltaTime * 25f);

			if (Vector2.Distance(head.transform.position, origin.position) < 0.1f)
			{
				playerController.chainAttackFinished = true;

				gameObject.SetActive(false);
			}
		}
	}


    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") || collider.CompareTag("PlayerHit")) return;

		if (goingForward) goingForward = false;

		// checks if enemy
		if (collider.CompareTag("Enemy"))
		{
			weapon.OnHit(gameObject, collider);
		}
	}

	void OnDisable()
	{
		playerController.chainAttackFinished = true;
	}
}
