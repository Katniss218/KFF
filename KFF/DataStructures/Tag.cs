
using System;

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
			ValidateName( name );

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
		
		internal static void ValidateName( string name )
		{
			if( string.IsNullOrEmpty( name ) )
			{
				throw new ArgumentNullException( "The name can't be null or empty." );
			}
			if( !(Syntax.IsAlphabetical( name[0] ) || name[0] == '_') )
			{
				throw new KFFParseException( "Expected to find [A-Za-z_], but found '" + name[0] + "' (char: " + 0 + ")." );
			}
			for( int i = 1; i < name.Length; i++ )
			{
				if( !(Syntax.IsAlphaNumerical( name[i] ) || name[i] == '_') )
				{
					throw new KFFParseException( "Expected to find [A-Za-z0-9_], but found '" + name[i] + "' (char: " + i + ")." );
				}
			}
		}
	}
}