
namespace KFF.DataStructures
{
	/// <summary>
	/// Represents a Payload that can hold string values.
	/// </summary>
	public sealed class PayloadString : Payload
	{
		internal string value { get; private set; }

		/// <summary>
		/// Returns the type of this Payload (Read Only).
		/// </summary>
		public override DataType type
		{
			get
			{
				return DataType.String;
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
		/// Creates a payload of type String with the specified value.
		/// </summary>
		/// <param name="value">The value to hold.</param>
		public PayloadString( string value )
		{
			this.value = value ?? throw new KFFException( "String payload must contain a non-null value." );
		}



		/// <summary>
		/// Converts a payload of type String into a string data type.
		/// </summary>
		/// <param name="other">The payload to convert.</param>
		public static implicit operator string( PayloadString other )
		{
			return other.value;
		}

		/// <summary>
		/// Convert a string data type into a payload of type String.
		/// </summary>
		/// <param name="other">The data type to convert.</param>
		public static implicit operator PayloadString( string other )
		{
			return new PayloadString( other );
		}

		public static bool operator ==( PayloadString left, PayloadString right )
		{
			return left.value.Equals( right.value );
		}

		public static bool operator !=( PayloadString left, PayloadString right )
		{
			return !left.value.Equals( right.value);
		}
	}
}