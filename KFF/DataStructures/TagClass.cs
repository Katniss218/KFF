
namespace KFF.DataStructures
{
	/// <summary>
	/// Represents a Tag that can hold other Tags.
	/// </summary>
	public sealed class TagClass : Tag, IClass
	{
		/// <summary>
		/// Contains the payload of this Tag.
		/// </summary>
		public PayloadClass payload { get; set; }

		/// <summary>
		/// Returns the type of this tag (Read only).
		/// </summary>
		public override DataType type
		{
			get
			{
				return this.payload.type;
			}
		}

		/// <summary>
		/// Returns the number of tags currently in this class (Read Only).
		/// </summary>
		public int count
		{
			get
			{
				return this.payload.count;
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
		/// Creates a new tag of type Class with the specified name and an empty payload.
		/// </summary>
		/// <param name="name">The name of the new Tag.</param>
		public TagClass( string name ) : base( name )
		{
			this.payload = new PayloadClass();
			this.payload.EmbedIn( this );
		}
		
		/// <summary>
		/// Creates a new tag of type Class with the specified name and payload.
		/// </summary>
		/// <param name="name">The name of the new Tag.</param>
		/// <param name="payload">The payload of the new Tag.</param>
		public TagClass( string name, PayloadClass payload ) : base( name )
		{
			this.payload = payload;
			this.payload.EmbedIn( this );
		}



		public static bool operator ==( TagClass left, TagClass right )
		{
			return left.name == right.name && left.payload == right.payload;
		}

		public static bool operator !=( TagClass left, TagClass right )
		{
			return left.name != right.name || left.payload != right.payload;
		}
		/// <summary>
		/// Checks if the class contains a tag with the specified name.
		/// </summary>
		/// <param name="name">The name to check.</param>
		public bool Has( string name )
		{
			return this.payload.Has( name );
		}

		/// <summary>
		/// Tries to get the tag with the specified name, as a generic Tag object. Returns true if the tag was found.
		/// </summary>
		/// <param name="name">The name of the tag to get.</param>
		/// <param name="t">The variable to store the tag into. Is going to be null if the operation fails.</param>
		public bool TryGet( string name, out Tag t )
		{
			return this.payload.TryGet( name, out t );
		}

		/// <summary>
		/// Gets the tag with the specified name, as a generic Tag object.
		/// </summary>
		/// <param name="name">The name of the tag to get.</param>
		/// <exception cref="KFFObjectPresenceException">Thrown when the tag with the specified name isn't present in the class.</exception>
		public Tag Get( string name )
		{
			return this.payload.Get( name );
		}

		/// <summary>
		/// Gets all the tags in the class, as a generic Tag object array.
		/// </summary>
		public Tag[] GetAll()
		{
			return this.payload.GetAll();
		}
		
		/// <summary>
		/// Sets the tags with the specified names. Replaces the tag if already present, adds a new tag otherwise.
		/// </summary>
		/// <param name="t">The new tags to add to the class.</param>
		public void Set( Tag[] t )
		{
			this.payload.Set( t );
		}

		/// <summary>
		/// Removes a tag with the specified name from the class.
		/// </summary>
		/// <param name="name">The name of the tag to remove.</param>
		public void Remove( string name )
		{
			this.payload.Remove( name );
		}

		/// <summary>
		/// Removes all tags from the class.
		/// </summary>
		public void Clear()
		{
			this.payload.Clear();
		}
	}
}