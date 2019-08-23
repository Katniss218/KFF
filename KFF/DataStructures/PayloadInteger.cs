
namespace KFF.DataStructures
{
	/// <summary>
	/// Represents a Payload that can hold 8-byte integer values.
	/// </summary>
	public sealed class PayloadInteger : Payload
	{
		internal long value { get; private set; }

		/// <summary>
		/// Returns the type of this Payload (Read Only).
		/// </summary>
		public override DataType type
		{
			get
			{
				return DataType.Integer;
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
		/// Creates a payload of type Integer with the specified value.
		/// </summary>
		/// <param name="value">The value to hold.</param>
		public PayloadInteger( long value )
		{
			this.value = value;
		}



		/// <summary>
		/// Converts a payload of type Integer into a long data type.
		/// </summary>
		/// <param name="other">The payload to convert.</param>
		public static implicit operator long( PayloadInteger other )
		{
			return other.value;
		}
		/// <summary>
		/// Convert a long data type into a payload of type Integer.
		/// </summary>
		/// <param name="other">The data type to convert.</param>
		public static implicit operator PayloadInteger( long other )
		{
			return new PayloadInteger( other );
		}
		public static bool operator ==( PayloadInteger left, PayloadInteger right )
		{
			return left.value == right.value;
		}

		public static bool operator !=( PayloadInteger left, PayloadInteger right )
		{
			return left.value != right.value;
		}
	}
}