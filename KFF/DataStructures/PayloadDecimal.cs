
namespace KFF.DataStructures
{
	/// <summary>
	/// Represents a Payload that can hold 8-byte IEEE-754 floating-point values.
	/// </summary>
	public sealed class PayloadDecimal : Payload
	{
		internal double value { get; private set; }

		/// <summary>
		/// Returns the type of this Payload (Read Only).
		/// </summary>
		public override DataType type
		{
			get
			{
				return DataType.Decimal;
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
		/// Creates a payload of type Decimal with the specified value.
		/// </summary>
		/// <param name="value">The value to hold.</param>
		public PayloadDecimal( double value )
		{
			this.value = value;
		}



		/// <summary>
		/// Converts a payload of type Decimal into a double data type.
		/// </summary>
		/// <param name="other">The payload to convert.</param>
		public static implicit operator double( PayloadDecimal other )
		{
			return other.value;
		}
		/// <summary>
		/// Convert a double data type into a payload of type Decimal.
		/// </summary>
		/// <param name="other">The data type to convert.</param>
		public static implicit operator PayloadDecimal( double other )
		{
			return new PayloadDecimal( other );
		}
		public static bool operator ==( PayloadDecimal left, PayloadDecimal right )
		{
			return left.value == right.value;
		}

		public static bool operator !=( PayloadDecimal left, PayloadDecimal right )
		{
			return left.value != right.value;
		}
	}
}
