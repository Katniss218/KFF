
namespace KFF.DataStructures
{
	/// <summary>
	/// Represents a tag that can hold 1-byte boolean values.
	/// </summary>
	public sealed class TagBoolean : Tag
	{
		/// <summary>
		/// Contains the payload of this tag.
		/// </summary>
		public PayloadBoolean payload { get; set; }

		/// <summary>
		/// Returns the type of this tag (Read Only).
		/// </summary>
		public override DataType type
		{
			get
			{
				return this.payload.type;
			}
		}

		/// <summary>
		/// Returns true if this Object is a Tag, false otherwise (Read Only).
		/// </summary>
		public override bool isTag
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Returns true if this Object is a Payload, false otherwise (Read Only).
		/// </summary>
		public override bool isPayload
		{
			get
			{
				return false;
			}
		}



		/// <summary>
		/// Creates a new tag of type Boolean with the specified name and payload.
		/// </summary>
		/// <param name="name">The name of the new Tag.</param>
		/// <param name="payload">The payload of the new Tag.</param>
		public TagBoolean( string name, PayloadBoolean payload ) : base( name )
		{
			this.payload = payload;
			this.payload.EmbedIn( this );
		}

		public static bool operator ==( TagBoolean left, TagBoolean right )
		{
			return left.name == right.name &&left.payload == right.payload;
		}

		public static bool operator !=( TagBoolean left, TagBoolean right )
		{
			return left.name != right.name || left.payload != right.payload;
		}
	}
}