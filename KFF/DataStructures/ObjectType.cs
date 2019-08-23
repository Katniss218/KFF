
namespace KFF.DataStructures
{
	/// <summary>
	/// Represents either a tag or a payload.
	/// </summary>
	public enum ObjectType : byte
	{
		/// <summary>
		/// A tag, tags have names.
		/// </summary>
		Tag,
		/// <summary>
		/// A payload, payloads don't have names.
		/// </summary>
		Payload
	}
}