using UnityEngine;

public class Vanity
{
	/// <summary>
	/// The type of body part, which matches the name of the folder the animator for this body part is in)
	/// </summary>
	public string type;

	/// <summary>
	/// The id of the specific body part, which matches the name of the animator the bodypart will use
	/// </summary>
	public string id;

	/// <summary>
	/// The color of the body part
	/// </summary>
	public Color color;

	/// <summary>
	///  Stores information needed for a character body part.
	/// </summary>
	/// <param name="typeInput">The type of body part, which matches the name of the folder the animator for this body part is in)</param>
	/// <param name="idInput">The id of the specific body part, which matches the name of the animator the bodypart will use</param>
	/// <param name="colorInput">The color of the body part</param>
	public Vanity(string typeInput, string idInput, Color colorInput)
	{
		type = typeInput;
		id = idInput;
		color = colorInput;
	}

	public class VanitySet
	{
		public Vanity body = new Vanity("Body", "LargeCharacter", new Color(0.85f, 0.62f, 0.47f));
		public Vanity hair = new Vanity("Hair", "Hair5", Color.black);
		public Vanity shirt = new Vanity("Shirts", "DefaultShirt", Color.white);
		public Vanity pants = new Vanity("Pants", "DefaultPants", new Color(0.17f, 0.24f, 1));

		/// <summary>
		/// Creates a set of body parts for the player
		/// </summary>
		public VanitySet(Vanity bodyInput = null, Vanity hairInput = null, Vanity shirtInput = null, Vanity pantsInput = null)
		{
			// sets properties to input, but only if applicable
			body = bodyInput == null ? body : bodyInput;
			hair = hairInput == null ? hair : hairInput;
			shirt = shirtInput == null ? shirt : shirtInput;
			pants = pantsInput == null ? pants : pantsInput;
		}
	}
}
