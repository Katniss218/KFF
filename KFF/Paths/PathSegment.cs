using KFF.DataStructures;
using System;

namespace KFF
{
	/// <summary>
	/// Represents a segment of a KFFPath.
	/// </summary>
	public struct PathSegment
	{
		/// <summary>
		/// Contains the name of the Tag that the path segment refers to, or a string representation of the index, null if the path goes backwards (Read Only).
		/// </summary>
		public readonly string name;

		/// <summary>
		/// Contains the index of the Payload that the path segment refers to, -1 if N/A (Read Only).
		/// </summary>
		public readonly int index;

		/// <summary>
		/// Contains the destination of the path segment. Will be 'ObjectType.Tag' if the segment is named, 'ObjectType.Payload' if it's indexed (Read Only).
		/// </summary>
		public readonly ObjectType destination;

		/// <summary>
		/// Contains the direction to take when traversing along the path. Forward to move deeper, Backward to move... back (Read Only).
		/// </summary>
		public readonly PathDirection direction;


		/// <summary>
		/// Creates a new path segment from a string and an array of placeholder values.
		/// </summary>
		public PathSegment( string s, int[] placeholderValues )
		{
			// A valid path segment is:
			// - A Tag identifier (name), or an index, or a placeholder to an index (indicates forward path segment).
			// - A '<' character (indicates backward path segment).
			if( string.IsNullOrEmpty( s ) )
			{
				// Segment is null or empty.
				throw new ArgumentNullException( "Path Segment can't be empty." );
			}
			//
			// backward path segment.
			//
			if( s[0] == Syntax.PATH_BACKWARD )
			{
				if( s.Length > 1 )
				{
					throw new KFFException( "Expected to find '" + Syntax.PATH_BACKWARD + "', but found '" + s + "'." );
				}
				this.direction = PathDirection.Backward;
				this.index = -1;
				this.name = null;
				this.destination = ObjectType.Payload; // N/A, but must be set to something.
			}
			//
			// Placeholder indexed path segment.
			//
			else if( s[0] == Syntax.PATH_PLACEHOLDER_OPENING )
			{
				// Index of placeholder can only be digits 0-9.
				for( int i = 1; i < s.Length - 1; i++ )
				{
					if( !Syntax.IsDigit( s[i] ) )
					{
						throw new KFFException( "Expected to find '" + Syntax.PATH_PLACEHOLDER_CLOSING + "', but found '" + s[i] + "' (char: " + i + ")." );
					}
				}
				if( s[s.Length - 1] == Syntax.PATH_PLACEHOLDER_CLOSING )
				{
					string number = s.Substring( 1, s.Length - 2 );

					int placeholderIndex = int.Parse( number );

					if( placeholderIndex >= placeholderValues.Length )
					{
						throw new KFFException( "The placeholder index '" + s + "' is outside of bounds of the placeholder values array (length: " + placeholderValues.Length + ")." );
					}

					this.direction = PathDirection.Forward;
					this.index = placeholderValues[placeholderIndex];
					this.name = placeholderValues[placeholderIndex].ToString( Syntax.numberFormat );
					this.destination = ObjectType.Payload;
				}
				else
				{
					throw new KFFException( "Expected to find '" + Syntax.PATH_PLACEHOLDER_CLOSING + "', but found '" + s[s.Length - 1] + "' (char: " + (s.Length - 2) + ")." );
				}
			}
			else
			{
				// Segment is a positive integer (indexed).
				if( int.TryParse( s, System.Globalization.NumberStyles.None, Syntax.numberFormat, out int o ) )
				{
					this.direction = PathDirection.Forward;
					this.index = o;
					this.name = s;
					this.destination = ObjectType.Payload;
				}
				// Segment is a Tag's identifier (named).
				else
				{
					Tag.ValidateName( s );

					this.direction = PathDirection.Forward;
					this.name = s;
					this.index = -1;
					this.destination = ObjectType.Tag;
				}
			}
		}

		public static bool operator ==( PathSegment left, PathSegment right )
		{
			return left.name == right.name
				&& left.index == right.index
				&& left.destination == right.destination
				&& left.direction == right.direction;
		}

		public static bool operator !=( PathSegment left, PathSegment right )
		{
			return left.name != right.name
				|| left.index != right.index
				|| left.destination != right.destination
				|| left.direction != right.direction;
		}

	}
}