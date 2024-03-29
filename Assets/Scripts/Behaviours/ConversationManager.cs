using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;
using static Conversation;

public class ConversationManager : MonoBehaviour
{
	const float namePadding = 15f;
	const float choiceSpacing = 10f;

	public TMP_Text textObject;
	public TMP_Text nameObject;
	public GameObject canvas;
	public GameObject continueArrow;
	public Transform choiceContainer;
	public GameObject choicePrefab;

	// animation settings
	List<int> shakeIndexes = new List<int>();
	List<int> wobbleIndexes = new List<int>();

	bool addChar = false;

	public void StartConversation(Conversation conversation, Animator speakerAnimator)
	{
		StartCoroutine(StartConversationCoroutine(conversation, speakerAnimator, true));
	}

	public void StartDialogue(Dialogue dialogue, Animator speakerAnimator)
	{
		FindObjectOfType<PlayerController>().controllable = false;

		StartCoroutine(StartDialogueCoroutine(dialogue, speakerAnimator));
	}

	IEnumerator StartConversationCoroutine(Conversation conversation, Animator speakerAnimator, bool finishAllConversations)
	{
		// takes away player control
		bool previouslyControllable = FindObjectOfType<PlayerController>().controllable;
		FindObjectOfType<PlayerController>().StopControl();

		foreach (Dialogue dialogue in conversation.conversation)
		{
			yield return StartCoroutine(StartDialogueCoroutine(dialogue, speakerAnimator));
		}

		if (finishAllConversations)
		{
			// gives back player control
			if (previouslyControllable) FindObjectOfType<PlayerController>().StartControl();

			// hides dialogue box
			gameObject.SetActive(false);
		}			
	}

	IEnumerator StartDialoguesCoroutine(Dialogue[] dialogues, Animator speakerAnimator)
	{
		foreach(Dialogue dialogue in dialogues)
		{
			yield return StartCoroutine(StartDialogueCoroutine(dialogue, speakerAnimator));
		}
	}

	IEnumerator StartDialogueCoroutine(Dialogue dialogue, Animator speakerAnimator)
	{
		// invokes onDialogueStart event
		if (dialogue.onDialogueStart != null)
		{
			dialogue.onDialogueStart.Invoke();
		}

		// reset effects
		shakeIndexes.Clear();
		wobbleIndexes.Clear();

		// hides continue arrow
		continueArrow.SetActive(false);

		// interpolates text if needed
		string interpolatedDialogue = dialogue.dialogue;

		if(dialogue.interpolatedText != null && dialogue.interpolatedText.Count > 0)
		{
			foreach(InterpolatedText interpolatedText in dialogue.interpolatedText)
			{
				interpolatedDialogue = interpolatedDialogue.Replace(interpolatedText.key, interpolatedText.valueCallback.InvokeWithCallback<string>());
			}
		}

		// parses text
		DialogueData dialogueData = GetDialogueData(interpolatedDialogue);

		// sets text and name
		textObject.text = dialogueData.TMPParsedText;
		if(dialogue.speakerName == null || dialogue.speakerName == "")
		{
			nameObject.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			nameObject.transform.parent.gameObject.SetActive(true);
			nameObject.text = dialogue.speakerName;

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

				while (timer < dialogue.interval)
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

					if (!skipDialogue) yield return null;
				}
			}
		}

		// makes sure all characters are shown at the end
		textObject.maxVisibleCharacters = interpolatedDialogue.Length;

		// checks if there is an attatched speaker animator and animation for when finished talking
		if (!!speakerAnimator && !!dialogue.doneTalkingClip)
		{
			speakerAnimator.Play(FindStateContainingClip(speakerAnimator, dialogue.doneTalkingClip).name);
		}

		// checks if there are choices attatched
		if (dialogue.choices != null && dialogue.choices.Length > 0)
		{
			Choice choiceMade = null;

			float totalHeight = dialogue.choices.Length * (choicePrefab.GetComponent<RectTransform>().rect.height + choiceSpacing);

			// loops through choices
			for (int i = 0; i < dialogue.choices.Length; i++)
			{
				// last to first so the last choice is at the bottom
				Choice choice = dialogue.choices[dialogue.choices.Length - 1 - i];

				// creates new choice object from prefab
				GameObject choiceObject = Instantiate(choicePrefab);
				choiceObject.transform.SetParent(choiceContainer, false);

				yield return 0;

				// places choice

				// centers on x axis
				choiceObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, i * (choiceObject.GetComponent<RectTransform>().sizeDelta.y + choiceSpacing));

				// labels choice
				choiceObject.transform.Find("Label").GetComponent<TMP_Text>().text = choice.text;

				// adds consequence function to onclick of choice
				choiceObject.GetComponent<Button>().onClick.AddListener(delegate
				{
					// marks that a choice has been chosen
					choiceMade = choice;
				});
			}

			// wait until a choice is made
			while (choiceMade == null)
			{
				yield return null;
			}

			// destroys choices
			foreach (Transform child in choiceContainer)
			{
				Destroy(child.gameObject);
			}

			// invokes onDialogueEnd event
			if (dialogue.onDialogueEnd != null)
			{
				dialogue.onDialogueEnd.Invoke();
			}

			// invokes onChose event
			if (choiceMade.onChoose != null)
			{
				choiceMade.onChoose.Invoke();
			}

			// plays attatched dialogues
			if (choiceMade.attatchedDialogues != null && choiceMade.attatchedDialogues.Length > 0)
			{
				yield return StartCoroutine(StartDialoguesCoroutine(choiceMade.attatchedDialogues, speakerAnimator));
			}

			// plays attatched conversation
			if (choiceMade.attatchedConversation != null)
			{
				yield return StartCoroutine(StartConversationCoroutine(choiceMade.attatchedConversation, speakerAnimator, false));
			}
		}
		else
		{
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

			// invokes onDialogueEnd event
			if (dialogue.onDialogueEnd != null)
			{
				dialogue.onDialogueEnd.Invoke();
			}

			// hides continue arrow
			continueArrow.SetActive(false);
		}
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
