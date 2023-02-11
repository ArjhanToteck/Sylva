using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class LoadItems : MonoBehaviour
{
	public Item.Items items;

	void Awake()
	{
		// loops through fields in items
		foreach(FieldInfo field in items.GetType().GetFields())
		{
			// gets local item
			object localItem = field.GetValue(items);

			// gets static item
			object staticItem = field.GetValue(Item.items);

			// loops through item fields
			foreach (FieldInfo field2 in staticItem.GetType().GetFields())
			{
				// checks if field marked as editable
				if (Attribute.IsDefined(field2, typeof(Item.EditableAttribute)))
				{
					// updates field
					field2.SetValue(staticItem, field2.GetValue(localItem));
				}
			}

			// updates item
			field.SetValue(Item.items, staticItem);

			/*// checks type of item
			var @switch = new Dictionary<Type, Action> {
				{
					typeof(Item.Weapon), () => {
						// gets localItem as a weapon
						Item.Weapon localWeapon = (Item.Weapon)localItem;

						// gets static weapon
						Item.Weapon staticWeapon = (Item.Weapon)field.GetValue(Item.items);

						// loops through weapon fields
						foreach(FieldInfo field2 in staticWeapon.GetType().GetFields())
						{
							// checks if field marked as editable
							if(Attribute.IsDefined(field2, typeof(Item.EditableAttribute)))
							{
								// updates field
								field2.SetValue(staticWeapon, field2.GetValue(localWeapon));
							}
						}

						// updates weapon
						field.SetValue(Item.items, staticWeapon);
					} 
				}
			};

			@switch[localItem.GetType()]();*/
		}
	}
}
