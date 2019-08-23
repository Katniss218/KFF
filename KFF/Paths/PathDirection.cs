
namespace KFF
{
	/// <summary>
	/// Represents a direction to take when traversing a path, either forward or back.
	/// </summary>
	public enum PathDirection : byte
	{
		/// <summary>
		/// The forwards direction moves deeper into the nested structure of the KFF data tree.
		/// </summary>
		Forward,
		/// <summary>
		/// The backwards direction moves back from the nested structure of the KFF data tree.
		/// </summary>
		Backward
	}
}
