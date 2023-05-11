using System;
using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Conversation", menuName = "ScriptableObject/Conversation", order = 1)]
public class Conversation : ScriptableObject
{
	/// <summary>
	/// If this event returns false, the conversation is not executed.
	/// </summary>
	public UltEvent executionCondition = null;

	public Dialogue[] conversation;

	[Serializable]
	public class Dialogue
	{
		/// <summary>
		/// If this event returns false, the dialogue is not executed.
		/// </summary>
		public UltEvent executionCondition = null;

		/// <summary>
		/// The actual spoken lines.
		/// </summary>
		public string dialogue = "";

		/// <summary>
		/// The name of the person speaking. Leave empty or null to hide the name box.
		/// </summary>
		public string speakerName = "";

		[SerializeField]
		public List<InterpolatedText> interpolatedText;

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

		/// <summary>
		/// This action is performed before this dialogue is shwon.
		/// </summary>
		public UltEvent onDialogueStart;

		/// <summary>
		/// This action is performed after this dialogue is shwon.
		/// </summary>
		public UltEvent onDialogueEnd;
	}

	[Serializable]
	public class Choice
	{
		/// <summary>
		/// This label is shown when presented this choice.
		/// </summary>
		public string text;

		/// <summary>
		/// This action is performed when this choice is made.
		/// </summary>
		public UltEvent onChoose;

		// will generally have one or the other, never both of these
		public Dialogue[] attatchedDialogues;
		public Conversation attatchedConversation;
	}

	[Serializable]
	public class InterpolatedText
	{
		public string key;

		public UltEvent valueCallback;
	}
}
