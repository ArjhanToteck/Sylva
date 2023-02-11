using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Header("External Scripts")]
	public HealthBar healthBar;
	public ItemSwitch itemSwitch;

	[Header("Body Parts")]
	public Parts parts;

	[Header("Components")]
	public CharacterController2D controller;
	public Rigidbody2D rigidbody;

	[Header("Motion Settings")]
	float xSpeed;
	public bool jumping = false;
	public bool falling = false;
	public bool justLanded = false;
	public bool hitCooldown = false;

	[Header("Attack Data")]
	public bool swingAttack = false;
	public bool inSwingAttackAnimation = false;
	public float swingAttackProgress = -1f; // this value is 0-1 for percent completion of an attack animation. 1 means finished attacking.

	public bool chainAttack = false;
	public bool chainAttackFinished = true;

	public float aimAngle = 0; // used for chain weapons and spells

	public Item.Weapon weaponSelectedBeforeAttack;

	[Header("Spell Data")]
	public bool castSpell = false;
	public float castSpellProgress = -1f; // this value is 0-1 for percent completion of a spell casting animation. 1 means finished casting.

	public Item.Spell spellSelectedBeforeAttack;

	[Header("Data")]
	public PlayerData data;

	[Serializable]
	public class Parts
	{
		public GameObject weapon;
		public GameObject spell;
		public GameObject hair;
		public GameObject body;
		public GameObject arms;
		public GameObject helmet;
		public GameObject shirt;
		public GameObject sleeves;
		public GameObject pants;
	}

	void Start()
	{
		// loads armor and clothing
		LoadArmor();

		// loads weapon
		LoadWeapon();

		// loads spell
		LoadSpell();

		// adjusts healthbar to start
		healthBar.setMaxHealth(data.maxHealth);
		healthBar.setHealth(data.health);
	}

	void Update()
	{
		// applies bounds to animation progress variables
		if (swingAttackProgress > 1) swingAttackProgress = -1;
		if (swingAttackProgress < 0) swingAttackProgress = -1;

		if (castSpellProgress > 1) castSpellProgress = -1;
		if (castSpellProgress < 0) castSpellProgress = -1;

		// switch weapon

		// checks if switch weapon button held
		if (Input.GetButton("SwitchWeapon") || Input.GetAxis("SwitchWeapon") > 0.5f)
		{
			// checks if not already switching an item
			if (!itemSwitch.switchingItem)
			{
				// sets parameters to switch
				itemSwitch.SwitchItem(new List<Item>(data.weapons), data.selectedWeapon, (int selection) =>
				{
					// sets selected weapon to the weapon that was selected just now
					data.selectedWeapon = selection;

					// loads weapon when ready
					StartCoroutine(LoadWeaponWhenReady());

					IEnumerator LoadWeaponWhenReady()
					{
						// waits until not attacking
						while(swingAttackProgress != -1 || !chainAttackFinished || castSpellProgress != -1)
						{
							yield return null;
						}

						// loads weapon
						LoadWeapon();
					}
				});
			}
		}
		// checks if switch spell button held
		else if (Input.GetButton("SwitchSpell") || Input.GetAxis("SwitchSpell") < -0.5f)
		{
			// checks if not already switching an item
			if (!itemSwitch.switchingItem)
			{
				// sets parameters to switch
				itemSwitch.SwitchItem(new List<Item>(data.spells), data.selectedSpell, (int selection) =>
				{
					// sets selected weapon to the weapon that was selected just now
					data.selectedSpell = selection;

					// loads spell when ready
					StartCoroutine(LoadSpellWhenReady());

					IEnumerator LoadSpellWhenReady()
					{
						// waits until not attacking
						while (swingAttackProgress != -1 || !chainAttackFinished || castSpellProgress != -1)
						{
							yield return null;
						}

						// loads spell
						LoadSpell();
					}
				});
			}
		}
		else
		{
			itemSwitch.switchingItem = false;
		}

		// only does these things while not paused
		if (Time.timeScale > 0)
		{
			// run

			// sets horizontal speed
			xSpeed = Input.GetAxisRaw("Horizontal") * data.speed;

			// jump

			// checks if jump button pressed
			if ((Input.GetButton("Jump") || Input.GetAxis("Jump") > 0.5f) && !justLanded && !falling)
			{
				jumping = true;
			}
			else
			{
				justLanded = false;
			}

			// attack

			// checks if attacking
			if ((Input.GetButton("Attack") || Input.GetAxis("Attack") > 0.5f) && chainAttackFinished && swingAttackProgress == -1 && castSpellProgress == -1)
			{
				// remembers weapon equipped at time of attack
				weaponSelectedBeforeAttack = data.weapons[data.selectedWeapon];

				// gets aimed angle
				aimAngle = GetAimAngle(Input.GetButton("Attack")); // if keyboard was used, button is true rather than axis

				// sets up attack

				// checks if chain weapon
				if (data.weapons[data.selectedWeapon].GetType() == typeof(Item.Weapon.ChainWeapon))
				{
					chainAttack = true;
				}
				// regular swing weapon
				else
				{
					swingAttackProgress = 0;
					swingAttack = true;
					inSwingAttackAnimation = true;
				}
			}
			else
			{
				swingAttack = false;
				chainAttack = false;
			}

			// spell
			if ((Input.GetButton("Spell") || Input.GetAxis("Spell") > 0.5f) && chainAttackFinished && swingAttackProgress == -1 && castSpellProgress == -1)
			{
				// remembers spell equipped at start of attack
				spellSelectedBeforeAttack = data.spells[data.selectedSpell];

				// marks spell as ready to cast
				castSpell = true;

				// gets aimed angle in case spell needs it
				aimAngle = GetAimAngle(Input.GetButton("Spell")); // if keyboard was used, button is true rather than axis
			}
			else
			{
				castSpell = false;
			}

				// falling

				// falling if not on ground and accelerating downards
				falling = !controller.grounded; //&& rb.velocity.y < -0.1;

			// loops through animators to set params
			foreach (FieldInfo fieldInfo in parts.GetType().GetFields())
			{
				GameObject part = (GameObject)fieldInfo.GetValue(parts);

				// gets animator of body part
				Animator animator = part.transform.childCount == 0 || !part.transform.GetChild(0).GetComponent<Animator>() ? part.GetComponent<Animator>() : part.transform.GetChild(0).GetComponent<Animator>();

				// sets animation parameters

				// general
				animator.SetFloat("horizontalSpeed", Mathf.Abs(xSpeed));
				animator.SetBool("jumping", jumping);
				animator.SetBool("falling", falling);

				animator.SetBool("hitCooldown", hitCooldown);

				// swing weapons
				animator.SetFloat("weaponSpeed", weaponSelectedBeforeAttack.speed);
				animator.SetBool("swingAttack", swingAttack);
				animator.SetFloat("swingAttackProgress", swingAttackProgress);

				// makes sure swing attack is started up if animation didn't finish
				if (swingAttackProgress != -1f)
				{
					if (data.weapons[data.selectedWeapon].GetType() == typeof(Item.Weapon)) animator.SetBool("swingAttack", true);
				}

				// chain weapons
				animator.SetBool("chainAttack", chainAttack);
				animator.SetBool("chainAttackFinished", chainAttackFinished);
				
				// spells
				animator.SetFloat("spellSpeed", spellSelectedBeforeAttack.speed);
				animator.SetBool("castSpell", castSpell);
				animator.SetFloat("castSpellProgress", castSpellProgress);

				// makes sure spell casting is started up if animation didn't finish
				if (castSpellProgress != -1f)
				{
					if (data.spells[data.selectedSpell].GetType() == typeof(Item.Spell)) animator.SetBool("castSpell", true);
				}
			}
		}
	}

	void FixedUpdate()
	{
		// moves player using parameters from before
		controller.Move(xSpeed * Time.deltaTime, false, jumping);

		// resets jumping variable
		jumping = false;
	}

	// turns player around without motion
	public void Flip()
	{
		controller.Flip();
	}

	public bool TakeHit(int damage, Vector2 knockback)
	{
		// doesn't do anything if in cooldown
		if(hitCooldown) return false;

		// knockback
		rigidbody.velocity = new Vector3(rigidbody.velocity.x, knockback.y);
		controller.Move(knockback.x, false, false);

		// TODO: make armor reduce damage

		// subtracts damage from health
		data.health -= damage;
		healthBar.setHealth(data.health);

		// hit cooldown
		StartCoroutine(WaitForHitCooldown());

		return true;
	}

	IEnumerator WaitForHitCooldown()
	{
		hitCooldown = true;

		yield return new WaitForSeconds(data.hitCooldown);

		hitCooldown = false;
	}

	public float GetAimAngle(bool keyboard)
	{
		float angle;

		// checks if keyboard was used
		if (keyboard)
		{
			// gets aim from mouse position relative to player
			Vector2 screenPos = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
			angle = Mathf.Atan2(screenPos.y, screenPos.x) * Mathf.Rad2Deg;
			if (angle < 0.0f) angle += 360f;
		}
		else
		{
			// gets aim from joystick
			angle = Mathf.Atan2(-Input.GetAxisRaw("AimGamepadY"), Input.GetAxisRaw("AimGamepadX")) * Mathf.Rad2Deg;
		}

		return angle;
	}

	public void LoadArmor()
	{
		// sets armor, hair, and skin

		// hair

		// sets hair sprite
		parts.hair.transform.GetChild(0);
		parts.hair.transform.GetChild(0).GetComponent<PlayerAnimatior>();
		parts.hair.transform.GetChild(0).GetComponent<PlayerAnimatior>().spriteSheet = data.hair;

		// checks if hair is supposed to be hidden by helmet
		if (data.hair != null && (data.armor.helmet == null || !data.armor.helmet.hideHair))
		{
			// shows hair
			parts.hair.GetComponent<SpriteRenderer>().enabled = true;

			// sets hair color
			parts.hair.GetComponent<SpriteRenderer>().color = data.hairColor;
		}
		else
		{
			// hides hair
			parts.hair.GetComponent<SpriteRenderer>().enabled = false;
		}

		// sets skin color
		parts.body.GetComponent<SpriteRenderer>().color = data.skinColor;
		parts.arms.GetComponent<SpriteRenderer>().color = data.skinColor;

		// armor

		// checks if player has helmet
		if (data.armor.helmet != null)
		{
			// removes old controller if applicable
			Transform oldController = parts.helmet.transform.Find("Controller");

			if (!!oldController)
			{
				Destroy(oldController.gameObject);
			}

			// adds controller as child if applicable
			if (data.armor.helmet.controller != null)
			{
				GameObject controllerChild = Instantiate(data.armor.helmet.controller, parts.helmet.transform);
				controllerChild.name = "Controller";
			}

			// shows helmet
			parts.helmet.GetComponent<SpriteRenderer>().enabled = true;

			// sets helmet sprite
			parts.helmet.transform.GetChild(0).GetComponent<PlayerAnimatior>().spriteSheet = data.armor.helmet.spriteSheet;

			// sets helmet color
			parts.helmet.GetComponent<SpriteRenderer>().color = data.armor.helmet.color;
		}
		else
		{
			// hides helmet
			parts.helmet.GetComponent<SpriteRenderer>().enabled = false;
		}

		// checks if player has shirt
		if (data.armor.shirt != null)
		{
			// removes old controller if applicable
			Transform oldController = parts.shirt.transform.Find("Controller");

			if (!!oldController)
			{
				Destroy(oldController.gameObject);
			}

			// adds controller as child if applicable
			if (data.armor.shirt.controller != null)
			{
				GameObject controllerChild = Instantiate(data.armor.shirt.controller, parts.shirt.transform);
				controllerChild.name = "Controller";
			}

			// shows shirt and sleeves
			parts.shirt.GetComponent<SpriteRenderer>().enabled = true;
			parts.sleeves.GetComponent<SpriteRenderer>().enabled = true;

			// sets shirt and sleeves sprite
			parts.shirt.transform.GetChild(0).GetComponent<PlayerAnimatior>().spriteSheet = data.armor.shirt.spriteSheet;
			parts.sleeves.transform.GetChild(0).GetComponent<PlayerAnimatior>().spriteSheet = data.armor.shirt.sleevesSpritesheet;

			// sets shirt and sleeves color
			parts.shirt.GetComponent<SpriteRenderer>().color = data.armor.shirt.color;
			parts.sleeves.GetComponent<SpriteRenderer>().color = data.armor.shirt.color;
		}
		else
		{
			// hides shirt and sleeves
			parts.shirt.GetComponent<SpriteRenderer>().enabled = false;
			parts.sleeves.GetComponent<SpriteRenderer>().enabled = false;
		}

		// checks if player has pants
		if (data.armor.pants != null)
		{
			// removes old controller if applicable
			Transform oldController = parts.pants.transform.Find("Controller");

			if (!!oldController)
			{
				Destroy(oldController.gameObject);
			}

			// adds controller as child if applicable
			if (data.armor.pants.controller != null)
			{
				GameObject controllerChild = Instantiate(data.armor.pants.controller, parts.pants.transform);
				controllerChild.name = "Controller";
			}

			// shows pants
			parts.pants.GetComponent<SpriteRenderer>().enabled = true;

			// sets pants sprite
			parts.pants.transform.GetChild(0).GetComponent<PlayerAnimatior>().spriteSheet = data.armor.pants.spriteSheet;

			// sets pants color
			parts.pants.GetComponent<SpriteRenderer>().color = data.armor.pants.color;
		}
		else
		{
			// hides pants
			parts.pants.GetComponent<SpriteRenderer>().enabled = false;
		}

		// loops through animators to update clothes
		foreach (FieldInfo fieldInfo in parts.GetType().GetFields())
		{
			GameObject part = (GameObject)fieldInfo.GetValue(parts);

			// gets animator of body part
			PlayerAnimatior animator = part.transform.GetChild(0).GetComponent<PlayerAnimatior>();

			// checks if player animator exists
			if (!!animator)
			{
				// forces script to update clothes
				animator.UpdateClothes();
			}
		}
	}

	public void LoadWeapon()
	{
		// TODO: rework for chain weapons
		// clears old events
		parts.weapon.transform.Find("SwingWeapon").GetComponent<SwingWeaponController>().ClearEvents();

		// removes old controller if applicable
		Transform oldController = parts.weapon.transform.Find("Controller");

		if (!!oldController)
		{
			Destroy(oldController.gameObject);
		}

		// adds controller as child if applicable
		if (data.weapons[data.selectedWeapon].controller != null)
		{
			GameObject controllerChild = Instantiate(data.weapons[data.selectedWeapon].controller, parts.weapon.transform);
			controllerChild.name = "Controller";
		}
	}

	public void LoadSpell()
	{
		// clears old events
		parts.spell.transform.Find("SpellController").GetComponent<SpellController>().ClearEvents();

		// removes old controller if applicable
		Transform oldController = parts.spell.transform.Find("Controller");

		if (!!oldController)
		{
			Destroy(oldController.gameObject);
		}

		// adds controller as child if applicable
		if (data.spells[data.selectedSpell].controller != null)
		{
			GameObject controllerChild = Instantiate(data.spells[data.selectedSpell].controller, parts.spell.transform);
			controllerChild.name = "Controller";
		}
	}
}
