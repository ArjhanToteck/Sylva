using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
	[Header("Game Objects")]
	public TMP_Text textObject;
	public TMP_Text nameObject;
	public GameObject canvas;

	[Header("Parameters")]
	public string dialogue = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea.";
	public string speakerName = "Name";
	public float duration = 5f;
	public float interval = 0.1f;

	// animation settings
	List<int> shakeIndexes = new List<int>();
	List<int> wobbleIndexes = new List<int>();

	bool addChar = false;

	void SetName(string name)
	{
		this.speakerName = name;
	}

	// Start is called before the first frame update
	void StartDialogue(AnimationEvent animationEvent)
	{
		canvas.SetActive(true);

		dialogue = animationEvent.stringParameter;
		duration = animationEvent.floatParameter;
		speakerName = animationEvent.intParameter > 0 ? speakerName : null;

		StartCoroutine(RevealText());
	}

	IEnumerator RevealText()
	{
		// parses text
		DialogueData dialogueData = GetDialogueData(dialogue);

		// sets text and name
		textObject.text = dialogueData.TMPParsedText;
		if(speakerName == null)
		{
			nameObject.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			nameObject.transform.parent.gameObject.SetActive(true);
			nameObject.text = speakerName;
		}

		// hides all characters
		textObject.maxVisibleCharacters = 0;
		/*for (int i = 0; i < textObject.textInfo.characterInfo.Length; i++)
		{
			TMP_CharacterInfo charInfo = textObject.textInfo.characterInfo[i];

			if (!charInfo.isVisible) continue;

			Vector3[] vertices = textObject.textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
			vertices[0] = new Vector2(0, 0);
			vertices[1] = new Vector2(0, 0);
			vertices[2] = new Vector2(0, 0);
			vertices[3] = new Vector2(0, 0);
		}

		// refreshes mesh
		for (int i = 0; i < textObject.textInfo.meshInfo.Length; i++)
		{
			TMP_MeshInfo meshInfo = textObject.textInfo.meshInfo[i];
			meshInfo.mesh.vertices = meshInfo.vertices;
			textObject.UpdateGeometry(meshInfo.mesh, i);
		}*/

		// loops through each chunk of text
		foreach (TextChunk chunk in dialogueData.chunks)
		{
			// loops through each character in chunk
			for (int i = 0; i < chunk.text.Length; i++)
			{
				int currentCharacter = chunk.start + i;

				// shows one more character
				//textObject.maxVisibleCharacters++;
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

				// waits before next character
				yield return new WaitForSeconds(interval);
			}
		}

		// waits for duration
		if(duration > 0) yield return new WaitForSeconds(duration);

		// marks dialogue as finished
		GetComponent<Animator>().SetTrigger("dialogueFinished");
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

				/*// different special parameters for certain tags
				switch (tag)
				{
					case "color":
					{
						string color = text.Substring(i).Split('>')[0].Split('=')[1];
						effect = new TextEffect(tag, color);

						break;
					}

					default:
					{
						effect = new TextEffect(tag);
						break;
					}
				}*/

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
					if (currentContent.Length > 0)
					{
						data.chunks.Add(new TextChunk(currentContent, TextEffect.CloneList(currentEffects)));
						data.plainText += currentContent;
						data.TMPParsedText += currentContent;

						// if not a custom effecct, it is a TMP effect and stays in TMPParsedText
						if (!TextEffect.customEffects.Contains(tag))
						{
							data.TMPParsedText += "</" + text.Substring(i).Split("</")[1].Split('>')[0] + ">";
						}
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

	public class DialogueData
	{
		public string rawText = "";
		public string TMPParsedText = "";
		public string plainText = "";
		public List<TextChunk> chunks = new List<TextChunk>();
	}
}
