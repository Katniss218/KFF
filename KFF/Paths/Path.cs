﻿using KFF.DataStructures;
using System.Text;

namespace KFF
{
	/// <summary>
	/// Used for selecting specific tags/payloads, inside of the KFFFile.
	/// <para>
	/// The Path consists of segments, each of which can be either a tag's name or an index to a list.
	/// </para>
	/// </summary>
	public struct Path
	{
		readonly PathSegment[] segments;

		internal PathSegment this[int nest]
		{
			get
			{
				return this.segments[nest];
			}
		}



		/// <summary>
		/// Contains the type of object that the path points to (Tag/Payload) (Read only).
		/// </summary>
		public readonly ObjectType destination;

		/// <summary>
		/// Returns the number of segments in the path (Read Only).
		/// </summary>
		public int nestLevel
		{
			get
			{
				return segments == null ? 0 : this.segments.Length;
			}
		}

		/// <summary>
		/// Returns true if the path is empty (doesn't point to anything), false otherwise (Read Only).
		/// </summary>
		public bool isEmpty
		{
			get
			{
				return this.segments == null || this.segments.Length == 0;
			}
		}



		internal Path( params PathSegment[] segments )
		{
			this.segments = segments;
			this.destination = this.segments[this.segments.Length - 1].destination;
		}


		/// <summary>
		/// Parses a new path from the string representation.
		/// </summary>
		/// <param name="s">The string to parse the path from.</param>
		/// <param name="placeholderValues">The values to replace placeholder symbols with.</param>
		public Path( string s, params int[] placeholderValues )
		{
			// Placeholders only exist in the input. Every path with placeholders can be represented using just indexed segment.

			// If the string is null or empty, create an empty path that points to the kff file itself.
			if( string.IsNullOrEmpty( s ) )
			{
				this.segments = null;
				this.destination = ObjectType.Tag;
			}
			// Otherwise, split the segments at the KFFSyntax.PATH_SEGMENT_SEPARATOR character and parse them separately.
			else
			{
				string[] arr = s.Split( Syntax.PATH_SEGMENT_SEPARATOR );

				this.segments = new PathSegment[arr.Length];

				for( int i = 0; i < arr.Length; i++ )
				{
					this.segments[i] = new PathSegment( arr[i], placeholderValues );
				}

				this.destination = this.segments[this.segments.Length - 1].destination;
			}
		}


		/// <summary>
		/// Returns the string representation of this path.
		/// </summary>
		public override string ToString()
		{
			// If the path is empty, return an empty string.
			if( this.isEmpty )
			{
				return "";
			}

			// Otherwise, loop through all of the segments and join them with the KFFSyntax.PATH_SEGMENT_SEPARATOR in between.
			StringBuilder sb = new StringBuilder();

			sb.Append( this.segments[0].name );
			for( int i = 1; i < this.segments.Length; i++ )
			{
				sb.Append( Syntax.PATH_SEGMENT_SEPARATOR );
				sb.Append( this.segments[i].name );
			}

			return sb.ToString();
		}

		/// <summary>
		/// Converts a string into a path, using the 'new Path( string )' constructor.
		/// </summary>
		public static implicit operator Path( string stringRepresentation )
		{
			return new Path( stringRepresentation );
		}

		public static bool operator ==( Path left, Path right )
		{
			if( left.segments.Length != right.segments.Length )
			{
				return false;
			}
			for( int i = 0; i < left.segments.Length; i++ )
			{
				if( left.segments[i] != right.segments[i] )
				{
					return false;
				}
			}
			return left.destination == right.destination;
		}

		public static bool operator !=( Path left, Path right )
		{
			if( left.segments.Length != right.segments.Length )
			{
				return true;
			}
			for( int i = 0; i < left.segments.Length; i++ )
			{
				if( left.segments[i] != right.segments[i] )
				{
					return true;
				}
			}
			return left.destination != right.destination;
		}
	}
}