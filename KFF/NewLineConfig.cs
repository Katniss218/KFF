
namespace KFF
{
	/// <summary>
	/// Used as a 2-bit mask to tell the KFFWriter whether or not it should place a whitespace/newline/etc. around a specified token.
	/// </summary>
	public enum BeforeAfterMask : byte
	{
		/// <summary>
		/// Don't place any characters.
		/// </summary>
		None = 0,
		/// <summary>
		/// Place the character only before the token.
		/// </summary>
		Before = 1,
		/// <summary>
		/// Place the character only after the token.
		/// </summary>
		After = 2,
		/// <summary>
		/// Place the character both before and after the token.
		/// </summary>
		Both = 3
	}
}