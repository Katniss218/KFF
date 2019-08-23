using KFF.DataStructures;
using System;

namespace KFF
{
	/// <summary>
	/// Represents a segment of a KFFPath.
	/// </summary>
	internal struct PathSegment
	{
		// Contains the name of the Tag that the path segment refers to, or a string representation of the index, null if the path goes backwards (Read Only).
		internal readonly string name;
		
		// Contains the index of the Payload that the path segment refers to, -1 if N/A (Read Only).
		internal readonly int index;

		// Contains the destination of the path segment. Will be 'ObjectType.Tag' if the segment is named, 'ObjectType.Payload' if it's indexed (Read Only).
		internal readonly ObjectType destination;

		// Contains the direction to take when traversing along the path. Forward to move deeper, Backward to move... back (Read Only).
		internal readonly PathDirection direction;


		// TODO ----- Add method for easily implementing placeholders for variables
		//   Function( 'Objects.0.Vertices.%', i )
		//       ( string path_signature, params int variables )


		// Creates a new path segment from string.
		internal PathSegment( string s )
		{
			// A valid path segment is:
			// - A Tag identifier (name), or an index (indicates forward path segment).
			// - A '<' character (indicates backward path segment).
			if( string.IsNullOrEmpty( s ) )
			{
				// Segment is null or empty.
				throw new ArgumentNullException( "Path Segment can't be empty." );
			}
			if( s[0] == Syntax.PATH_BACKWARD )
			{
				if( s.Length > 1 )
				{
					throw new KFFException( "Expected \"<\", but found \"" + s + "\"." );
				}
				this.direction = PathDirection.Backward;
				this.index = -1;
				this.name = null;
				this.destination = ObjectType.Payload; // N/A, but must be set to something.
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
					KFFValidator.ValidateName( s );

					this.direction = PathDirection.Forward;
					this.name = s;
					this.index = -1;
					this.destination = ObjectType.Tag;
				}
			}
		}
	}
}