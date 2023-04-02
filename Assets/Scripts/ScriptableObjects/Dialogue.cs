using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Dialogue", order = 1)]
public class Dialogue : ScriptableObject
{
	/// <summary>
	/// The name of the person speaking. Leave empty or null to hide the name box.
	/// </summary>
	public string speakerName = "";

	/// <summary>
	/// The actual spoken lines.
	/// </summary>
	public string dialogue = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea.";

	/// <summary>
	/// The time, in seconds, between every letter showing up.
	/// </summary>
	public float interval = 0.1f;

	/// <summary>
	/// An array of choices to be presented after the dialogue is finished.
	/// </summary>
	public string[] choices;

	/// <summary>
	/// The animation clip to be played by the speaker while talking.
	/// </summary>
	public AnimationClip talkingClip;

	/// <summary>
	/// The animation clip to be played by the speaker when finished talking.
	/// </summary>
	public AnimationClip doneTalkingClip;
}
