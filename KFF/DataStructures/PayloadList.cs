using System;
using System.Collections.Generic;

namespace KFF.DataStructures
{
	/// <summary>
	/// Represents a Payload that can hold a list of Payloads (each of the same type).
	/// </summary>
	public sealed class PayloadList : Payload, IList
	{
		internal List<Payload> value { get; private set; }

		/// <summary>
		/// Contains the type of values held by the list (Read only).
		/// </summary>
		public DataType listType { get; private set; }

		/// <summary>
		/// Returns the type of the Payload. NOT the type of values held by the list (Read Only).
		/// </summary>
		public override DataType type
		{
			get
			{
				return DataType.List;
			}
		}

		/// <summary>
		/// Returns the number of Payloads currently in the list (Read Only).
		/// </summary>
		public int count
		{
			get
			{
				return this.value.Count;
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
		/// Creates an empty payload of type List.
		/// </summary>
		public PayloadList()
		{
			this.listType = DataType.EmptyList;
			this.value = new List<Payload>();
		}



		public static bool operator ==( PayloadList left, PayloadList right )
		{
			if( left.count != right.count )
				return false;
			for( int i = 0; i < left.count; i++ )
				if( left.value[i] != right.value[i] )
					return false;
			return true;
		}

		public static bool operator !=( PayloadList left, PayloadList right )
		{
			if( left.count != right.count )
				return true;
			for( int i = 0; i < left.count; i++ )
				if( left.value[i] != right.value[i] )
					return true;
			return false;
		}
		/// <summary>
		/// Checks if the list contains a payload at the specified index.
		/// </summary>
		/// <param name="index">The index to check.</param>
		public bool Has( int index )
		{
			return index >= 0 && index < this.count;
		}

		/// <summary>
		/// Tries to get the payload at the specified index, as a generic Payload object. Returns true if the payload was found.
		/// </summary>
		/// <param name="index">The index of the payload to get.</param>
		/// <param name="output">The variable to store the payload into. Is going to be null if the operation fails.</param>
		public bool TryGet( int index, out Payload output )
		{
			if( index < 0 || index > this.count )
			{
				output = null;
				return false;
			}
			output = this.value[index];
			return true;
		}

		/// <summary>
		/// Gets the payload at the specified index, as a generic Payload object.
		/// </summary>
		/// <param name="index">The index of the payload to get.</param>
		/// <exception cref="KFFObjectPresenceException">Thrown when the index is outside of the list's bounds.</exception>
		public Payload Get( int index )
		{
			if( index < 0 || index > this.count )
			{
				throw new KFFObjectPresenceException( "The index '" + index + "' isn't inside of the list." );
			}
			return this.value[index];
		}

		/// <summary>
		/// Gets all the payloads in the list, as a generic Payload object.
		/// </summary>
		public Payload[] GetAll()
		{
			return this.value.ToArray();
		}
		
		/// <summary>
		/// Adds an array of payloads to the end of the list.
		/// </summary>
		/// <param name="payload">The new payloads to add to the list.</param>
		/// <exception cref="ArgumentNullException">Thrown when the 'p' is null or 'p.Length' is 0.</exception>
		/// <exception cref="InvalidCastException">Thrown when the payload's type doesn't match the list's type.</exception>
		public void Add( params Payload[] payload )
		{
			if( payload == null || payload.Length == 0 )
			{
				throw new ArgumentNullException( "The payload can't be null or of length 0." );
			}
			if( this.listType == DataType.EmptyList )
			{
				this.listType = payload[0].type;
			}
			for( int i = 0; i < payload.Length; i++ )
			{
				if( payload[i].type != this.listType )
				{
					throw new InvalidCastException( "The payload type of payload no. " + i + " doesn't match the type of payload in the list." );
				}
				this.value.Add( payload[i] );
				payload[i].parent = this;
			}
		}

		/// <summary>
		/// Removes a payload at the specified index from the list. Sets the content type to EmptyList, if there are no payloads left.
		/// </summary>
		/// <param name="index">The index to remove the payload from.</param>
		/// <exception cref="KFFObjectPresenceException">Thrown when the index is outside of the list's bounds.</exception>
		public void Remove( int index )
		{
			if( index < 0 || index > this.count )
			{
				throw new KFFObjectPresenceException( "The index '" + index + "' isn't inside of the list." );
			}
			this.value.RemoveAt( index );
			if( this.value.Count == 0 )
			{
				this.listType = DataType.EmptyList;
			}
		}

		/// <summary>
		/// Removes all payloads from the list. Sets the content type to EmptyList.
		/// </summary>
		public void Clear()
		{
			this.value.Clear();
			this.listType = DataType.EmptyList;
		}
	}
}