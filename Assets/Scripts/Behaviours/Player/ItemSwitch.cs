using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSwitch : MonoBehaviour
{
	[Header("Game Objects")]
	public GameObject contents;
	public GameObject itemsContainer;
	public GameObject itemPrefab;
	public TMP_Text label;

	public bool switchingItem = false;
	public bool previouslySwitchingItem = false;

	List<Item> items;
	int originalSelection;
	Action<int> callback;

	int currentSelection;
	bool buttonDown = false;

	float waitBeforeQuickChange = 0.6f;
	bool quickChanging = true;
	float quickChangeTime = 0.1f;

	private void Start()
	{
		// prerenders shit so it isn't slow (something to do with tmp makes it slow asf otherwise)
		contents.SetActive(true);
		contents.SetActive(false);
	}

	public void SwitchItem(List<Item> items, int originalSelection, Action<int> callback)
	{
		// doesn't switch more than once at a time
		if (switchingItem) return;

		// pauses time
		Time.timeScale = 0;

		// marks as currenty switching an item
		switchingItem = true;
		previouslySwitchingItem = true;

		// shows item switching menu
		contents.SetActive(true);

		// stores params
		this.items = items;
		this.originalSelection = originalSelection;
		currentSelection = originalSelection;
		this.callback = callback;

		if(items.Count > 0) ShowItems();
	}

	void ShowItems()
	{
		// deletes children of item container
		for (int i = 0; i < itemsContainer.transform.childCount; i++)
		{
			Destroy(itemsContainer.transform.GetChild(i).gameObject);
		}

		// sets label
		label.text = items[currentSelection].name;

		// loops through each item
		for (int i = 0; i < items.Count; i++)
		{
			// gets item
			Item item = items[i];

			// creates game object for item
			GameObject itemObject = Instantiate(itemPrefab, itemsContainer.transform);

			// sets name
			itemObject.name = item.name;

			// sets sprite
			itemObject.transform.Find("Icon").GetComponent<Image>().sprite = item.icon;

			// sets position
			float position = (i - currentSelection) * 65;
			itemObject.GetComponent<RectTransform>().localPosition = new Vector3(position, itemObject.GetComponent<RectTransform>().localPosition.y);

			// makes non-selected items slightly smaller
			if (i != currentSelection)
			{
				itemObject.GetComponent<RectTransform>().localScale = new Vector2(0.9f, 0.9f);
			}
		}
	}

	void Update()
	{
		// checks if barely stopped switching item
		if(!switchingItem && previouslySwitchingItem)
		{
			// makes sure this is only called once after switching an item
			previouslySwitchingItem = false;

			// hides item switching menu
			contents.SetActive(false);

			// deletes children of item container
			for(int i = 0; i < itemsContainer.transform.childCount; i++)
			{
				Destroy(itemsContainer.transform.GetChild(i).gameObject);
			}

			// calls callback with newly selected item index
			callback(currentSelection);

			// sets time back to normal
			Time.timeScale = 1;
		}

		// checks if switching item currently
		if (switchingItem)
		{
			if(items.Count > 0)
			{
				// choosing item to right
				if (Input.GetAxisRaw("SwitchItemDirection") > 0.75f)
				{
					// checks if already moving and not yet quick changing
					if (buttonDown && !quickChanging)
					{
						// waits for a second before moving quickly again
						waitBeforeQuickChange -= Time.unscaledDeltaTime;

						// returns if quick change wait isn't over
						if (waitBeforeQuickChange > 0)
						{
							return;
						}
						else
						{
							quickChanging = true;
							quickChangeTime = 0f;
						}
					}

					// checks if quick changing
					if (quickChanging)
					{
						// lowers timer
						if (quickChangeTime > 0f)
						{
							quickChangeTime -= Time.unscaledDeltaTime;
							return;
						}
						else
						{
							quickChangeTime = 0.1f;
						}
					}

					buttonDown = true;

					// selects next item if not last item
					if (currentSelection < items.Count - 1) currentSelection += 1;

					// refreshes UI
					ShowItems();
				}
				// choosing item to left
				else if (Input.GetAxisRaw("SwitchItemDirection") < -0.75f)
				{
					// checks if already moving and not yet quick changing
					if (buttonDown && !quickChanging)
					{
						// waits for a second before moving quickly again
						waitBeforeQuickChange -= Time.unscaledDeltaTime;

						// returns if quick change wait isn't over
						if (waitBeforeQuickChange > 0)
						{
							return;
						}
						else
						{
							quickChanging = true;
							quickChangeTime = 0f;
						}
					}

					// checks if quick changing
					if (quickChanging)
					{
						// lowers timer
						if (quickChangeTime > 0f)
						{
							quickChangeTime -= Time.unscaledDeltaTime;
							return;
						}
						else
						{
							quickChangeTime = 0.1f;
						}
					}

					buttonDown = true;

					// selects previous item if not first item
					if (currentSelection > 0) currentSelection -= 1;

					// refreshes UI
					ShowItems();
				}
				else
				{
					quickChanging = false;
					buttonDown = false;
					waitBeforeQuickChange = 0.6f;
				}
			}			
		}
	}
}
