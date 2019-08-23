using System;

namespace KFF.DataStructures
{
	/// <summary>
	/// Represents a Tag that can hold a list of Payloads (each of the same type).
	/// </summary>
	public sealed class TagList : Tag, IList
	{
		/// <summary>
		/// Contains the Payload of this tag.
		/// </summary>
		public PayloadList payload { get; set; }

		/// <summary>
		/// Returns the type of values held by this list (Read Only).
		/// </summary>
		public DataType listType
		{
			get
			{
				return payload.listType;
			}
		}

		/// <summary>
		/// Returns the type of this Tag (Read Only).
		/// </summary>
		public override DataType type
		{
			get
			{
				return payload.type;
			}
		}

		/// <summary>
		/// Returns the number of Payloads currently in this list (Read Only).
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
		/// Creates a new tag of type List with the specified name and an empty payload.
		/// </summary>
		/// <param name="name">The name of the new Tag.</param>
		public TagList( string name ) : base( name )
		{
			this.payload = new PayloadList();
			this.payload.EmbedIn( this );
		}
		
		/// <summary>
		/// Creates a new tag of type List with the specified name and payload.
		/// </summary>
		/// <param name="name">The name of the new Tag.</param>
		/// <param name="payload">The payload of the new Tag.</param>
		public TagList( string name, PayloadList payload ) : base( name )
		{
			this.payload = payload;
			this.payload.EmbedIn( this );
		}



		public static bool operator ==( TagList left, TagList right )
		{
			return left.name == right.name && left.payload == right.payload;
		}

		public static bool operator !=( TagList left, TagList right )
		{
			return left.name != right.name || left.payload != right.payload;
		}
		/// <summary>
		/// Checks if the list contains a payload at the specified index.
		/// </summary>
		/// <param name="index">The index to check.</param>
		public bool Has( int index )
		{
			return this.payload.Has( index );
		}

		/// <summary>
		/// Tries to get the payload at the specified index, as a generic Payload object. Returns true if the payload was found.
		/// </summary>
		/// <param name="index">The index of the payload to get.</param>
		/// <param name="p">The variable to store the payload into. Is going to be null if the operation fails.</param>
		public bool TryGet( int index, out Payload p )
		{
			return this.payload.TryGet( index, out p );
		}

		/// <summary>
		/// Gets the payload at the specified index, as a generic Payload object.
		/// </summary>
		/// <param name="index">The index of the payload to get.</param>
		/// <exception cref="KFFObjectPresenceException">Thrown when the index is outside of the list's bounds.</exception>
		public Payload Get( int index )
		{
			return this.payload.Get( index );
		}

		/// <summary>
		/// Gets all the payloads in the list, as a generic Payload object.
		/// </summary>
		public Payload[] GetAll()
		{
			return this.payload.GetAll();
		}

		/// <summary>
		/// Adds an array of payloads to the end of the list.
		/// </summary>
		/// <param name="p">The new payloads to add to the list.</param>
		/// <exception cref="ArgumentNullException">Thrown when the 'p' is null or 'p.Length' is 0.</exception>
		public void Add( params Payload[] p )
		{
			this.payload.Add( p );
		}

		/// <summary>
		/// Removes a payload at the specified index from the list. Sets the content type to EmptyList, if there are no payloads left.
		/// </summary>
		/// <param name="index">The index to remove the payload from.</param>
		/// <exception cref="KFFObjectPresenceException">Thrown when the index is outside of the list's bounds.</exception>
		public void Remove( int index )
		{
			this.payload.Remove( index );
		}

		/// <summary>
		/// Removes all payloads from the list. Sets the content type to EmptyList.
		/// </summary>
		public void Clear()
		{
			this.payload.Clear();
		}
	}
}