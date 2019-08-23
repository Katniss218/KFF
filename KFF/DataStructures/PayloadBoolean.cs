
namespace KFF.DataStructures
{
	/// <summary>
	/// Represents a Payload that can hold 1-byte boolean values.
	/// </summary>
	public sealed class PayloadBoolean : Payload
	{
		internal bool value { get; private set; }

		/// <summary>
		/// Returns the type of this Payload (Read Only).
		/// </summary>
		public override DataType type
		{
			get
			{
				return DataType.Boolean;
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
		/// Creates a Payload of type Boolean with the specified value.
		/// </summary>
		/// <param name="value">The value to hold.</param>
		public PayloadBoolean( bool value )
		{
			this.value = value;
		}



		/// <summary>
		/// Converts a Payload of type Boolean into a bool data type.
		/// </summary>
		/// <param name="other">The Payload to convert.</param>
		public static implicit operator bool( PayloadBoolean other )
		{
			return other.value;
		}
		/// <summary>
		/// Convert a bool data type into a payload of type Boolean.
		/// </summary>
		/// <param name="other">The data type to convert.</param>
		public static implicit operator PayloadBoolean( bool other )
		{
			return new PayloadBoolean( other );
		}
		public static bool operator ==( PayloadBoolean left, PayloadBoolean right )
		{
			return left.value == right.value;
		}

		public static bool operator !=( PayloadBoolean left, PayloadBoolean right )
		{
			return left.value != right.value;
		}
	}
}