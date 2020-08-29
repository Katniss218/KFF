using System.Collections.Generic;

namespace KFF.DataStructures
{
	/// <summary>
	/// Represents a Payload that can hold other Tags.
	/// </summary>
	public sealed class PayloadClass : Payload, IClass
	{
		internal List<Tag> value { get; private set; }

		/// <summary>
		/// Returns the type of this Payload (Read Only).
		/// </summary>
		public override DataType type
		{
			get
			{
				return DataType.Class;
			}
		}

		/// <summary>
		/// Returns the number of Tags currently in this class (Read only).
		/// </summary>
		public int count
		{
			get
			{
				return value.Count;
			}
		}

		/// <summary>
		/// Returns true if this Object is a Tag, false otherwise (Read Only).
		/// </summary>
		public override bool isTag
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Returns true if this Object is a Payload, false otherwise (Read Only).
		/// </summary>
		public override bool isPayload
		{
			get
			{
				return true;
			}
		}



		/// <summary>
		/// Creates an empty payload of type Class.
		/// </summary>
		public PayloadClass()
		{
			this.value = new List<Tag>();
		}



		public static bool operator ==( PayloadClass left, PayloadClass right )
		{
			if( left.count != right.count )
				return false;
			for( int i = 0; i < left.count; i++ )
				if( left.value[i] != right.value[i] )
					return false;
			return true;
		}

		public static bool operator !=( PayloadClass left, PayloadClass right )
		{
			if( left.count != right.count )
				return true;
			for( int i = 0; i < left.count; i++ )
				if( left.value[i] != right.value[i] )
					return true;
			return false;
		}


		/// <summary>
		/// Checks if the class contains a tag with the specified name.
		/// </summary>
		/// <param name="name">The name to check.</param>
		public bool Has( string name )
		{
			for( int i = 0; i < value.Count; i++ )
			{
				// NOTE! - Operator '==' zajmuje o 30% dłużej niż metoda 'Equals()'.
				if( string.Equals( value[i].name, name ) )
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Tries to get the tag with the specified name, as a generic Tag object. Returns true if the tag was found.
		/// </summary>
		/// <param name="name">The name of the tag to get.</param>
		/// <param name="t">The variable to store the tag into. Is going to be null if the operation fails.</param>
		public bool TryGet( string name, out Tag t )
		{
			for( int i = 0; i < value.Count; i++ )
			{
				if( string.Equals( value[i].name, name ) )
				{
					t = value[i];
					return true;
				}
			}
			t = null;
			return false;
		}
		
		/// <summary>
		/// Returns the index of the tag with the specified name.
		/// </summary>
		/// <param name="name">The name of the tag to search for.</param>
		public int IndexOf( string name )
		{
			for( int i = 0; i < value.Count; i++ )
			{
				if( string.Equals( value[i].name, name ) )
				{
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// Gets the tag with the specified name, as a generic Tag object.
		/// </summary>
		/// <param name="name">The name of the tag to get.</param>
		/// <exception cref="KFFObjectPresenceException">Thrown when the tag with the specified name isn't present in the class.</exception>
		public Tag Get( string name )
		{
			for( int i = 0; i < value.Count; i++ )
			{
				if( string.Equals( value[i].name, name ) )
				{
					return value[i];
				}
			}
			throw new KFFObjectPresenceException( "A tag with the name '" + name + "' doesn't exists in the class." );
		}

		/// <summary>
		/// Gets all the tags in the class, as a generic Tag object array.
		/// </summary>
		public Tag[] GetAll()
		{
			return this.value.ToArray();
		}
		
		/// <summary>
		/// Sets the tags with the specified names. Replaces the tag if already present, adds a new tag otherwise.
		/// </summary>
		/// <param name="t">The new tags to add to the class.</param>
		public void Set( params Tag[] t )
		{
			for( int i = 0; i < t.Length; i++ )
			{
				int index = IndexOf( t[i].name );
				if( index == -1 )
				{
					this.value.Add( t[i] );
					t[i].parent = this;
					continue;
				}
				this.value[index] = t[i];
				t[i].parent = this;
			}
		}

		/// <summary>
		/// Removes a tag with the specified name from the class.
		/// </summary>
		/// <param name="name">The name of the tag to remove.</param>
		public void Remove( string name )
		{
			for( int i = 0; i < value.Count; i++ )
			{
				if( string.Equals( value[i].name, name ) )
				{
					this.value.RemoveAt( i );
					return;
				}
			}
		}

		/// <summary>
		/// Removes all tags from the class.
		/// </summary>
		public void Clear()
		{
			this.value.Clear();
		}
	}
}