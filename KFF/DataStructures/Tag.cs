
namespace KFF.DataStructures
{
	/// <summary>
	/// Represents a generic tag.
	/// </summary>
	public abstract class Tag : Object
	{
		/// <summary>
		/// Contains the name (identifier) of this Tag.
		/// </summary>
		public string name { get; set; }
		
		/// <summary>
		/// Creates a new tag.
		/// </summary>
		/// <param name="name">The name of the new tag.</param>
		protected Tag( string name )
		{
			KFFValidator.ValidateName( name );

			this.name = name;
		}

		public static bool operator ==( Tag left, Tag right )
		{
			return left.name == right.name;
		}

		public static bool operator !=( Tag left, Tag right )
		{
			return left.name != right.name;
		}
	}
}