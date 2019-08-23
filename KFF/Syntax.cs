using System;
using System.Globalization;

namespace KFF
{
	/// <summary>
	/// Is used to hold characters for tokens. Also has several methods related to them.
	/// </summary>
	public static class Syntax
	{
		/// <summary>
		/// The "Name/Payload Separator" token.
		/// </summary>
		public const char NAME_PAYLOAD_SEPARATOR = '=';

		/// <summary>
		/// The "Tag End" token.
		/// </summary>
		public const char TAG_END = ';';

		/// <summary>
		/// The "Negative Sign" token.
		/// </summary>
		public const char NEGATIVE_SIGN = '-';

		/// <summary>
		/// The "Decimal Separator" token.
		/// </summary>
		public const char DECIMAL_SEPARATOR = '.';

		/// <summary>
		/// The "String Container" token.
		/// </summary>
		public const char STRING_CONTAINER = '\"'; // - '"'

		/// <summary>
		/// The "Escape Char" token.
		/// </summary>
		public const char ESCAPE_CHAR = '\\'; // - '\'

		public const char COMMENT_CHAR = '/'; // two of those make a comment beginning
		public const string COMMENT_TOKEN = "//";
		/// <summary>
		/// The "Class Opening" token.
		/// </summary>
		public const char CLASS_OPENING = '{';

		/// <summary>
		/// The "Class Closing" token.
		/// </summary>
		public const char CLASS_CLOSING = '}';

		/// <summary>
		/// The "List Opening" token.
		/// </summary>
		public const char LIST_OPENING = '[';

		/// <summary>
		/// The "List CLosing" token.
		/// </summary>
		public const char LIST_CLOSING = ']';

		/// <summary>
		/// The "List Element Separator" token.
		/// </summary>
		public const char LIST_ELEMENT_SEPARATOR = ',';

		/// <summary>
		/// The char used to separate path segments in the string representation.
		/// </summary>
		public const char PATH_SEGMENT_SEPARATOR = '.';
		
		/// <summary>
		/// The char used to indicate a path segment going backward, instead of usual forward.
		/// </summary>
		public const char PATH_BACKWARD = '<';

		/// <summary>
		/// The keyword for 'true' Boolean value.
		/// </summary>
		public const string TOKEN_TRUE = "true";

		/// <summary>
		/// The keyword for 'false' Boolean value.
		/// </summary>
		public const string TOKEN_FALSE = "false";

		/// <summary>
		/// The keyword for 'NaN' Decimal value.
		/// </summary>
		public const string TOKEN_NOT_A_NUMBER = "NaN";

		/// <summary>
		/// The keyword for the 'PositiveInfinity' and 'NegativeInfinity' Decimal values (prefixed with NEGATIVE_SIGN to form 'NegativeInfinity').
		/// </summary>
		public const string TOKEN_INFINITY = "Infinity";
		
		/// <summary>
		/// The format used to write numbers (decimal separator, negative sign and no groups).
		/// </summary>
		public static NumberFormatInfo numberFormat = new NumberFormatInfo()
		{
			NumberDecimalSeparator = DECIMAL_SEPARATOR.ToString(),
			NegativeSign = NEGATIVE_SIGN.ToString(),
			PositiveSign = ""
		};

		public static NumberStyles numberStyle = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent;

		/// <summary>
		/// Checks whether or not the given character has to be escaped when converting the Payload to the string representation. To convert it, call 'KFFSyntax.GetEscaped( char )'.
		/// </summary>
		/// <returns>
		/// Returns true if the character has be escaped, false otherwise.
		/// </returns>
		/// <param name="c">The character to check.</param>
		public static bool IsEscapable( char c )
		{
			// char:  '\' or '"'
			return c == ESCAPE_CHAR || c == STRING_CONTAINER;
		}

		/// <summary>
		/// Converts an escaped sequence of characters ('\\', '\"') into the real character that is being escaped by it.
		/// </summary>
		/// <param name="escapedSequence">The escaped sequence (must be exact, case sensitive).</param>
		/// <exception cref="ArgumentException">Thrown when the escaped sequence is not valid.</exception>
		public static char GetUnescaped( string escapedSequence )
		{
			// - '"'
			if( escapedSequence == ESCAPE_CHAR.ToString() + STRING_CONTAINER )
			{
				return STRING_CONTAINER;
			}
			// - '\'
			if( escapedSequence == ESCAPE_CHAR.ToString() + ESCAPE_CHAR )
			{
				return ESCAPE_CHAR;
			}
			throw new ArgumentException( "Can't get char. for an invalid escaped sequence." );
		}

		/// <summary>
		/// Converts an unescaped character ('\', '"') into the escaped sequence, that can be used in string representation, to represent it.
		/// </summary>
		/// <param name="unescapedChar">The unescaped char (must be exact, case sensitive).</param>
		/// <exception cref="ArgumentException">Thrown when the unescaped char is not escapable.</exception>
		public static string GetEscaped( char unescapedChar )
		{
			// - '"'
			if( unescapedChar == STRING_CONTAINER )
			{
				return ESCAPE_CHAR.ToString() + STRING_CONTAINER;
			}
			// - '\'
			if( unescapedChar == ESCAPE_CHAR )
			{
				return ESCAPE_CHAR.ToString() + ESCAPE_CHAR;
			}
			throw new ArgumentException( "Can't get escaped sequence for an invalid char." );
		}

		/// <summary>
		/// Checks whether or not the given char is a number (0-9).
		/// </summary>
		/// <returns>
		/// True if the char is a number (0-9), false otherwise.
		/// </returns>
		/// <param name="c">The char to check.</param>
		public static bool IsDigit( char c )
		{
			return c == '0' || c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' || c == '9';
		}

		/// <summary>
		/// Checks whether or not the given char is alphabetical (A-Z, a-z).
		/// </summary>
		/// <returns>
		/// True if the char is a valid letter (A-Z, a-z), false otherwise.
		/// </returns>
		/// <param name="c">The char to check.</param>
		public static bool IsAlphabetical( char c )
		{
			// Uppercase
			return c == 'Q' || c == 'W' || c == 'E' || c == 'R' || c == 'T' || c == 'Y' || c == 'U' || c == 'I' || c == 'O' || c == 'P' ||
				   c == 'A' || c == 'S' || c == 'D' || c == 'F' || c == 'G' || c == 'H' || c == 'J' || c == 'K' || c == 'L' ||
				   c == 'Z' || c == 'X' || c == 'C' || c == 'V' || c == 'B' || c == 'N' || c == 'M' ||
				   // Lowercase
				   c == 'q' || c == 'w' || c == 'e' || c == 'r' || c == 't' || c == 'y' || c == 'u' || c == 'i' || c == 'o' || c == 'p' ||
				   c == 'a' || c == 's' || c == 'd' || c == 'f' || c == 'g' || c == 'h' || c == 'j' || c == 'k' || c == 'l' ||
				   c == 'z' || c == 'x' || c == 'c' || c == 'v' || c == 'b' || c == 'n' || c == 'm';
		}

		/// <summary>
		/// Checks whether or not the given char is a white space.
		/// </summary>
		/// <returns>
		/// True if the char is a valid white space (HorizontalTabulation, LineFeed, CarriageReturn, Space), false otherwise
		/// </returns>
		/// <param name="c">The char to check</param>
		public static bool IsWhiteSpace( char c )
		{
			int cInt = c;
			// HorizontalTabulation, LineFeed, CarriageReturn, Space
			return cInt == 9 || cInt == 10 || cInt == 13 || cInt == 32; // char.IsWhiteSpace( c );
		}

		/// <summary>
		/// Checks whether or not the given char is alphanumerical (A-Z, a-z, 0-9). 
		/// </summary>
		/// <returns>
		/// True if the char is a valid letter (A-Z, a-z, 0-9), false otherwise.
		/// </returns>
		/// <param name="c">The char to check.</param>
		public static bool IsAlphaNumerical( char c )
		{
			return IsAlphabetical( c ) || IsDigit( c );
		}

		/// <summary>
		/// Gets the valid white space characters.
		/// </summary>
		/// <returns>
		/// A string containing every valid white space character.
		/// </returns>
		public static string GetValidWhiteSpaces()
		{
			return "\t\n\r ";
		}

		/// <summary>
		/// Gets the valid chars that can be at the beginning of the tag's name.
		/// </summary>
		/// <returns>
		/// A string containing every character that can be used at the beginning of a tag's name.
		/// </returns>
		public static string GetValidStartingChars()
		{
			return "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm_";
		}

		/// <summary>
		/// Gets the valid chars that can be in the middle/end of the tag's name.
		/// </summary>
		/// <returns>
		/// A string containing every character that can be used in the middle/end of a tag's name.
		/// </returns>
		public static string GetValidChars()
		{
			return "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm_0123456789";
		}

		/// <summary>
		/// Gets the valid chars that can be used to index the tag's position using a path.
		/// </summary>
		/// <returns>
		/// A string containing every decimal digit (0-9).
		/// </returns>
		public static string GetValidPathIndexers()
		{
			return "0123456789";
		}
	}
}