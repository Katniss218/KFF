﻿
namespace KFF.DataStructures
{
	/// <summary>
	/// Represents a Tag that can hold string values.
	/// </summary>
	public sealed class TagString : Tag
	{
		/// <summary>
		/// Contains the payload of this Tag.
		/// </summary>
		public PayloadString payload { get; set; }

		/// <summary>
		/// Returns the type of this Tag (Read Only).
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
		/// Creates a new tag of type String with the specified name and payload.
		/// </summary>
		/// <param name="name">The name of the new Tag.</param>
		/// <param name="payload">The payload of the new Tag.</param>
		public TagString( string name, PayloadString payload ) : base( name )
		{
			this.payload = payload;
			payload.EmbedIn( this );
		}
		public static bool operator ==( TagString left, TagString right )
		{
			return left.name == right.name && left.payload == right.payload;
		}

		public static bool operator !=( TagString left, TagString right )
		{
			return left.name != right.name || left.payload != right.payload;
		}
	}
}