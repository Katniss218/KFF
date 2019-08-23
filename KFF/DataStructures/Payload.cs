
namespace KFF.DataStructures
{
	/// <summary>
	/// Represents a generic payload.
	/// </summary>
	public abstract class Payload : Object
	{
		// Is the payload a part of some other structure? (Tag, file, etc.)
		internal bool isEmbedded { get; private set; }

		

		/// <summary>
		/// Creates a new payload.
		/// </summary>
		protected Payload() { }



		// Embeds a payload into a tag (DOESN'T DO ANY TYPE CHECKS).
		internal void EmbedIn( Tag tagToEmbedIn )
		{
			this.isEmbedded = true;
			this.parent = tagToEmbedIn;
		}

		// Embeds a payload into a tag (DOESN'T DO ANY TYPE CHECKS).
		internal void EmbedIn( KFFFile fileToEmbedIn )
		{
			this.isEmbedded = true;
			this.parent = fileToEmbedIn;
		}
	}
} // #.#e+# #.#e-# -#.#e+# -#.#e-#