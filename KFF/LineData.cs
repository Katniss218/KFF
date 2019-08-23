using System;

namespace KFF
{
	/// <summary>
	/// Used to represent line and collumn numbers.
	/// </summary>
	internal struct LineData
	{
		// <summary>
		// The line number.
		// </summary>
		internal int lineNo { get; set; }
		// <summary>
		// The collumn number.
		// </summary>
		internal int colNo { get; set; }

		// <summary>
		// Creates a new LineData with the specified lineNo and colNo.
		// </summary>
		// <param name="lineNo">The line number.</param>
		// <param name="colNo">The collumn number.</param>
		internal LineData( int lineNo, int colNo )
		{
			this.lineNo = lineNo;
			this.colNo = colNo;
		}

		// <summary>
		// Calculates the LineData's lineNo and colNo using a string and the position in that string.
		// </summary>
		// <param name="s">The string to check.</param>
		// <param name="pos">The position along the string to check.</param>
		internal static LineData Calculate( string s, int pos )
		{
			int newLineChars = 1; // beginning at line no. 1, not 0
			int charsSinceNewLine = 1; // beginning at col no. 1, not 0
			string newLine = Environment.NewLine;
			for( int i = 0; i < pos; i++ )
			{
				charsSinceNewLine++;
				if( s.Substring( i, newLine.Length ) == newLine )
				{
					i += newLine.Length;
					newLineChars++;
					charsSinceNewLine = 0;
				}
			}
			return new LineData( newLineChars, charsSinceNewLine );
		}

		/// <summary>
		/// Converts the LineData object into a string representation.
		/// </summary>
		public override string ToString()
		{
			return "line: " + this.lineNo + ", col: " + this.colNo;
		}
	}
}