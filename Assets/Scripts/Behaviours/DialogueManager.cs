using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
	const float namePadding = 15f;

	[Header("Game Objects")]
	public TMP_Text textObject;
	public TMP_Text nameObject;
	public GameObject canvas;
	public GameObject continueArrow;
	public GameObject choicePrefab;

	[Header("Parameters")]
	public string speakerName = "";
	public string dialogue = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea.";
	public float interval = 0.1f;
	public Animator speakerAnimator;

	[SerializeField]
	public DialogueData dialogueData;

	// animation settings
	List<int> shakeIndexes = new List<int>();
	List<int> wobbleIndexes = new List<int>();

	bool addChar = false;

	// lots of helper functions for unity events. fuck unity events.

	public void SetSpeakerName(string speakerName)
	{
		this.speakerName = speakerName;
	}

	public void SetDialogue(string dialogue)
	{
		this.dialogue = dialogue;
	}

	public void SetInterval(float interval)
	{
		this.interval = interval;
	}

	public void SetSpeakerAnimator(Animator speakerAnimator)
	{
		this.speakerAnimator = speakerAnimator;
	}

	public void StartConversation(Conversation conversation)
	{
		StartCoroutine(StartConversationCoroutine(conversation));
	}

	public IEnumerator StartConversationCoroutine(Conversation conversation)
	{
		// takes away player control
		bool previouslyControllable = FindObjectOfType<PlayerController>().controllable;
		FindObjectOfType<PlayerController>().StopControl();

		foreach (Dialogue dialogue in conversation.conversation)
		{
			yield return StartCoroutine(StartDialogueCoroutine(dialogue));
		}

		// gives back player control
		if (previouslyControllable) FindObjectOfType<PlayerController>().StartControl();

		// hides dialogue box
		gameObject.SetActive(false);
	}

	public void StartDialogue(Dialogue dialogue)
	{
		FindObjectOfType<PlayerController>().controllable = false;

		StartCoroutine(StartDialogueCoroutine(dialogue));
	}

	IEnumerator StartDialogueCoroutine(Dialogue dialogue)
	{
		// hides continue arrow
		continueArrow.SetActive(false);

		// parses text
		dialogueData = GetDialogueData(dialogue.dialogue);

		// sets text and name
		textObject.text = dialogueData.TMPParsedText;
		if(speakerName == null || speakerName == "")
		{
			nameObject.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			nameObject.transform.parent.gameObject.SetActive(true);
			nameObject.text = speakerName;

			// resizes text box
			float width = nameObject.preferredWidth * nameObject.GetComponent<RectTransform>().localScale.x + namePadding;

			nameObject.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(width, nameObject.transform.parent.GetComponent<RectTransform>().sizeDelta.y);
		}

		// hides all characters
		textObject.maxVisibleCharacters = 0;

		// checks if there is an attatched speaker animator and talking animation
		if (!!speakerAnimator && !!dialogue.talkingClip)
		{
			speakerAnimator.Play(FindStateContainingClip(speakerAnimator, dialogue.talkingClip).name);
		}

		// vars to track dialogue skip
		bool skipDialogue = false;
		bool buttonDown = false;
		bool buttonUp = false;

		// loops through each chunk of text
		foreach (TextChunk chunk in dialogueData.chunks)
		{
			// loops through each character in chunk
			for (int i = 0; i < chunk.text.Length; i++)
			{
				int currentCharacter = chunk.start + i;

				// shows one more character
				addChar = true;

				// shake
				if (TextEffect.IncludesEffect(chunk.effects, "shake"))
				{
					shakeIndexes.Add(currentCharacter);
				}

				// wobble
				if (TextEffect.IncludesEffect(chunk.effects, "wobble"))
				{
					wobbleIndexes.Add(currentCharacter);
				}

				// waits before next character or dialogue is skipped
				float timer = 0f;

				while (timer < interval)
				{
					timer += Time.deltaTime;

					if (Input.GetButtonDown("SkipDialogue"))
					{
						buttonDown = true;
					}

					if (Input.GetButtonUp("SkipDialogue"))
					{
						buttonUp = true;
					}

					skipDialogue = buttonDown && buttonUp;

					if (skipDialogue) break;

					yield return null;
				}

				if (skipDialogue) break;
			}

			if (skipDialogue) break;
		}

		// ensures all characters are shown at the end
		textObject.maxVisibleCharacters = dialogue.dialogue.Length;

		// checks if there is an attatched speaker animator and animation for when finished talking
		if (!!speakerAnimator && !!dialogue.doneTalkingClip)
		{
			speakerAnimator.Play(FindStateContainingClip(speakerAnimator, dialogue.doneTalkingClip).name);
		}

		// shows continue arrow
		continueArrow.SetActive(true);

		// waits a frame before checking for input to prevent accidentally skipping the entire thing without reading
		yield return null;

		// waits for input (press skip dialogue button)
		while (!Input.GetButtonDown("SkipDialogue"))
		{
			yield return null;
		}

		while (!Input.GetButtonUp("SkipDialogue"))
		{
			yield return null;
		}

		// hides continue arrow
		continueArrow.SetActive(false);
	}

	void Update()
	{
		// no need to do anything if canvas is disabled
		if (!canvas.activeInHierarchy) return;

		// changing maxVisibleCharacters before doing a mesh update fixes a weird jittering issue. idk why that is, but ill just leave it.
		if (addChar)
		{
			addChar = false;
			textObject.maxVisibleCharacters++;
		}

		// force update on tmp
		textObject.ForceMeshUpdate();

		// loops through every character to be wobbled
		foreach (int i in wobbleIndexes)
		{
			TMP_CharacterInfo charInfo = textObject.textInfo.characterInfo[i];
			Vector3[] vertices = textObject.textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

			if(!charInfo.isVisible) continue;

			// loops through all 4 vertices in char mesh
			for(int j = 0; j < 4; j++)
			{
				Vector3 origin = vertices[charInfo.vertexIndex + j];
				vertices[charInfo.vertexIndex + j] = origin + new Vector3(0, Mathf.Sin(Time.time * 3f + origin.x * 0.05f) * 5f, 0);
			}
		}

		// loops through every character to be shaken
		foreach (int i in shakeIndexes)
		{
			TMP_CharacterInfo charInfo = textObject.textInfo.characterInfo[i];
			Vector3[] vertices = textObject.textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

			if (!charInfo.isVisible) continue;

			float perlinX = Mathf.PerlinNoise(Time.time * 7 + i * 1, 0) * 6f;
			float perlinY = Mathf.PerlinNoise(Time.time * 7 + i * 1, 5) * 6f;

			// loops through all 4 vertices in char mesh
			for (int j = 0; j < 4; j++)
			{
				Vector3 origin = vertices[charInfo.vertexIndex + j];
				vertices[charInfo.vertexIndex + j] = origin + new Vector3(perlinX, perlinY, 0);
			}
		}

		// refreshes mesh
		for (int i = 0; i < textObject.textInfo.meshInfo.Length; i++)
		{
			TMP_MeshInfo meshInfo = textObject.textInfo.meshInfo[i];
			meshInfo.mesh.vertices = meshInfo.vertices;
			textObject.UpdateGeometry(meshInfo.mesh, i);
		}
	}

	DialogueData GetDialogueData(string text)
	{
		DialogueData data = new DialogueData();

		data.rawText = text;

		// keeps track of effects
		List<TextEffect> currentEffects = new List<TextEffect>();

		string currentContent = "";

		// loops through characters in text
		for (int i = 0; i < text.Length; i++)
		{
			// opening tag
			if (text[i] == '<' && text[i + 1] != '/')
			{
				// gets tag name
				string tag = text.Substring(i).Split('<')[1].Split('>')[0].Split('=')[0];

				// adds content before tag
				if (currentContent.Length > 0)
				{
					data.chunks.Add(new TextChunk(currentContent, TextEffect.CloneList(currentEffects)));
					data.plainText += currentContent;
					data.TMPParsedText += currentContent;
				}

				currentContent = "";

				TextEffect effect = new TextEffect(tag);

				// adds new effect
				currentEffects.Add(effect);

				// if not a custom effecct, it is a TMP effect and stays in TMPParsedText
				if (!TextEffect.customEffects.Contains(tag))
				{
					data.TMPParsedText += "<" + text.Substring(i).Split('<')[1].Split('>')[0] + ">";
				}

				// gets index of end of opening tag
				int openingEnd = i + text.Substring(i + 1).IndexOf('>');

				// skips to after opening tag
				i = openingEnd + 1;

				// continues loop
				continue;
			}
			else if (i < text.Length - 1 && text.Substring(i, 2) == "</")
			{
				// gets tag name
				string tag = text.Substring(i).Split("</")[1].Split('>')[0].Split('=')[0];

				bool effectFound = false;
				int j;

				// loops through current effects backwards
				for (j = currentEffects.Count - 1; j >= 0; j--)
				{
					// checks if tag matches an already existing effect
					if (currentEffects[j].effectName == tag)
					{
						effectFound = true;

						break;
					}
				}

				// checks if matching start tag was found earlier
				if (effectFound)
				{
					// adds content before tag
					data.chunks.Add(new TextChunk(currentContent, TextEffect.CloneList(currentEffects)));
					data.plainText += currentContent;
					data.TMPParsedText += currentContent;

					// if not a custom effecct, it is a TMP effect and stays in TMPParsedText
					if (!TextEffect.customEffects.Contains(tag))
					{
						data.TMPParsedText += "</" + text.Substring(i).Split("</")[1].Split('>')[0] + ">";
					}

					currentContent = "";

					// removes effect from list
					currentEffects.RemoveAt(j);

					// gets index of end of closing tag
					int closingEnd = i + text.Substring(i + 1).IndexOf('>');

					// skips to after closing tag
					i = closingEnd + 1;

					// continues loop
					continue;
				}
			}

			// if at this point, this is just regular content
			currentContent += text[i];
		}

		// adds content at end without any tag after
		if (currentContent.Length > 0)
		{
			data.chunks.Add(new TextChunk(currentContent, TextEffect.CloneList(currentEffects)));
			data.plainText += currentContent;
			data.TMPParsedText += currentContent;
		}

		// sets starts and ends
		int start = 0;
		foreach(TextChunk chunk in data.chunks)
		{
			chunk.start = start;
			chunk.end = start + chunk.text.Length - 1;

			start = chunk.end + 1;
		}

		return data;
	}

	AnimatorState FindStateContainingClip(Animator animator, AnimationClip clip)
	{
		AnimatorController ac = animator.runtimeAnimatorController as AnimatorController;

		// gets the root state machine for the Animator controller
		AnimatorStateMachine rootStateMachine = ac.layers[0].stateMachine;

		// searches the root state machine recursively for the animation clip
		AnimatorState state = FindStateContainingClipRecursive(rootStateMachine, clip);

		return state;

		AnimatorState FindStateContainingClipRecursive(AnimatorStateMachine stateMachine, AnimationClip clip)
		{
			// searches the current state machine for the animation clip
			foreach (ChildAnimatorState state in stateMachine.states)
			{
				if (state.state.motion == clip)
				{
					return state.state;
				}
			}

			// searches child state machines recursively for the animation clip
			foreach (ChildAnimatorStateMachine childStateMachine in stateMachine.stateMachines)
			{
				AnimatorState state = FindStateContainingClipRecursive(childStateMachine.stateMachine, clip);
				if (state != null)
				{
					return state;
				}
			}

			return null;
		}
	}

	[Serializable]
	public class TextChunk
	{
		public int start;
		public int end;

		public string text;
		public List<TextEffect> effects;

		public TextChunk(string text, List<TextEffect> effects = null)
		{
			this.text = text;
			this.effects = effects != null ? effects : new List<TextEffect>() { };
		}
	}

	[Serializable]
	public class TextEffect
	{
		public static List<string> customEffects = new List<string>(new string[] { "shake", "wobble" });

		public string effectName;
		//public string colorCode;
		//public float speed;

		public TextEffect(string effectName)
		{
			this.effectName = effectName;
			//this.colorCode = colorCode;
			//this.speed = speed;
		}

		public TextEffect Clone()
		{
			return new TextEffect(effectName);
		}

		public static List<TextEffect> CloneList(List<TextEffect> list)
		{
			List<TextEffect> result = new List<TextEffect>();

			for (int i = 0; i < list.Count; i++)
			{
				result.Add(list[i].Clone());
			}

			return result;
		}

		public static bool IncludesEffect(List<TextEffect> list, string name)
		{
			foreach(TextEffect effect in list)
			{
				if (effect.effectName == name) return true;
			}

			return false;
		}
	}

	[Serializable]
	public class DialogueData
	{
		public string rawText = "";
		public string TMPParsedText = "";
		public string plainText = "";
		public List<TextChunk> chunks = new List<TextChunk>();
	}
}
