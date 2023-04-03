using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Conversation", menuName = "ScriptableObjects/Conversation", order = 1)]
public class Conversation : ScriptableObject
{
	public Dialogue[] conversation;

	[Serializable]
	public class Dialogue
	{
		/// <summary>
		/// The actual spoken lines.
		/// </summary>
		public string dialogue = "";

		/// <summary>
		/// The name of the person speaking. Leave empty or null to hide the name box.
		/// </summary>
		public string speakerName = "";

		/// <summary>
		/// The time, in seconds, between every letter showing up.
		/// </summary>
		public float interval = 0.05f;

		/// <summary>
		/// An array of choices to be presented after the dialogue is finished.
		/// </summary>
		public Choice[] choices;

		/// <summary>
		/// The animation clip to be played by the speaker while talking.
		/// </summary>
		public AnimationClip talkingClip;

		/// <summary>
		/// The animation clip to be played by the speaker when finished talking.
		/// </summary>
		public AnimationClip doneTalkingClip;
	}

	[Serializable]
	public class Choice
	{
		/// <summary>
		/// This label is shown when presented this choice.
		/// </summary>
		public string text;

		/// <summary>
		/// This action will be performed if this choice is made.
		/// </summary>
		public UnityAction action;

		// will generally have one or the other, never both of these
		public Dialogue[] attatchedDialogues;
		public Conversation attatchedConversation;
	}
}
