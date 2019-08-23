
namespace KFF.DataStructures
{
	/// <summary>
	/// Represents a KFF data type. AABB
	/// </summary>
	public enum DataType : sbyte
	{
		/// <summary>
		/// Special data type, used when the value can't be represented by any tag.
		/// </summary>
		Invalid = -1,

		/// <summary>
		/// Special data type, only used to indicate that a List is empty.
		/// </summary>
		EmptyList = 0,

		/// <summary>
		/// Boolean data type, can hold a boolean (true/false).
		/// </summary>
		Boolean = 1,

		/// <summary>
		/// Numeric data type, can hold any 64-bit, signed integer.
		/// </summary>
		Integer = 2,

		/// <summary>
		/// Numeric data type, can hold any 64-bit, signed IEE 754 floating-point number.
		/// </summary>
		Decimal = 3,

		/// <summary>
		/// Text data type, can hold an arbitrarily large array of chars.
		/// </summary>
		String = 4,

		/// <summary>
		/// Compound data type, can hold a list of Tags.
		/// </summary>
		Class = 5,

		/// <summary>
		/// Compound data type, can hold a list of Payloads, each of the same type.
		/// </summary>
		List = 6
	}
}
