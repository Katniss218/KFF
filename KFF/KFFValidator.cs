using KFF.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace KFF
{
	/// <summary>
	/// Class containing methods for checking validity of a kff-string.
	/// </summary>
	public sealed class KFFValidator
	{
		// The string being checked.
		private string s;
		// Current 'header' position along the string.
		private int pos;
		// Current char, throws an EOF exception, if pos >= s.Length
		private char currentChar
		{
			get
			{
				if( this.pos >= this.s.Length )
				{
					throw new KFFEndOfFileException( "Unexpected end of file." );
				}
				return this.s[this.pos];
			}
		}

		/// <summary>
		/// Creates a validator using an unvalidated string.
		/// </summary>
		/// <param name="unvalidatedString">The string to validate.</param>
		public KFFValidator( string unvalidatedString )
		{
			this.s = unvalidatedString;
			this.pos = 0;
		}
		
		private void ValidateName( out string name )
		{
			if( Syntax.IsAlphabetical( currentChar ) || currentChar == '_' ) // Nazwy TAG'ów zaczynają się literą lub '_'.
			{
				StringBuilder sb = new StringBuilder();
				do
				{
					sb.Append( currentChar );
					pos++;
				} while( Syntax.IsAlphaNumerical( currentChar ) || currentChar == '_' );

				name = sb.ToString();
				return;
			}
			else
			{
				throw new KFFParseException( "Expected to find [A-Za-z_], but found '" + currentChar + "' (" + LineData.Calculate( this.s, pos ) + ")." );
			}
		}
		
		private void ValidatePayload( out DataType type )
		{
			int payloadStart = pos;

			// ----------
			//   CLASS
			// ----------
			if( currentChar == Syntax.CLASS_OPENING )
			{
				//Debug.Log( "beginning of a class " + pos + " " + currentChar);
				pos++;
				this.ValidateClass();

				this.SkipWhiteSpaces();

				if( currentChar == Syntax.CLASS_CLOSING )
				{
					pos++;
				}
				else
				{
					throw new KFFParseException( "Expected to find '" + Syntax.CLASS_CLOSING + "', but found '" + currentChar + "' (" + LineData.Calculate( this.s, this.pos ) + ")." );
				}
				type = DataType.Class;
				return;
			}

			// ----------
			//   LIST
			// ----------
			if( currentChar == Syntax.LIST_OPENING )
			{
				pos++;

				this.ValidateList();

				SkipWhiteSpaces();

				if( currentChar == Syntax.LIST_CLOSING )
				{
					pos++;
				}
				else
				{
					throw new KFFParseException( "Expected to find '" + Syntax.LIST_CLOSING + "', but found '" + currentChar + "' (" + LineData.Calculate( this.s, this.pos ) + ")." );
				}
				type = DataType.List;
				return;
			}

			// ----------
			//   STRING
			// ----------
			if( currentChar == Syntax.STRING_CONTAINER )
			{
				do
				{
					if( currentChar == Syntax.ESCAPE_CHAR )
					{
						pos++;
						if( Syntax.IsEscapable( currentChar ) )// currentChar == KFFSyntax.STRING_CONTAINER )
						{
							pos++;
						}
						else
						{
							throw new KFFParseException( "The char. '" + currentChar + "' isn't escapable (" + LineData.Calculate( this.s, this.pos ) + ")." );
						}
					}
					else
					{
						pos++;
					}
				} while( currentChar != Syntax.STRING_CONTAINER ); // To coś ustawi głowicę na zamykającym cudzysłowiu.

				pos++;
				type = DataType.String;
				return;
			}

			// ----------
			//   BOOLEAN
			// ----------
			if( currentChar == Syntax.TOKEN_TRUE[0] )
			{
				pos++;
				for( int i = 1; i < Syntax.TOKEN_TRUE.Length; pos++, i++ )
				{
					// If any of the chars is not valid, throw an exception.
					if( currentChar != Syntax.TOKEN_TRUE[i] )
					{
						throw new KFFParseException( "Expected to find '" + Syntax.TOKEN_TRUE[i] + "', but found '" + currentChar + "' (" + LineData.Calculate( this.s, this.pos ) + ")." );
					}
				}
				// If no exception was thrown, that means there is a 'true' boolean value.
				type = DataType.Boolean;
				return;
			}
			if( currentChar == Syntax.TOKEN_FALSE[0] )
			{
				pos++;
				for( int i = 1; i < Syntax.TOKEN_FALSE.Length; pos++, i++ )
				{
					// If any of the chars is not valid, throw an exception.
					if( currentChar != Syntax.TOKEN_FALSE[i] )
					{
						throw new KFFParseException( "Expected to find '" + Syntax.TOKEN_FALSE[i] + "', but found '" + currentChar + "' (" + LineData.Calculate( this.s, this.pos ) + ")." );
					}
				}
				// If no exception was thrown, that means there is a 'true' boolean value.
				type = DataType.Boolean;
				return;
			}
			
			// ----------
			//   Number
			// ----------
			if( currentChar == Syntax.NEGATIVE_SIGN )
			{
				pos++;
			}
			if( Syntax.IsDigit( currentChar ) ) // tagi numeryczne zaczynają się liczbą lub minusem w wypadku ujemnego tagu.
			{
				type = DataType.Integer;

				// pomiń wszystkie liczby
				do
				{
					pos++;
				}
				while( pos < s.Length && Syntax.IsDigit( currentChar ) );
				
				// jeśli liczby się skończyły, ale trafiło na kropkę (separator dziesiętny).
				if( currentChar == Syntax.DECIMAL_SEPARATOR )
				{
					type = DataType.Decimal;

					pos++;

					// pomiń kropkę i skipnij resztę liczb.
					if( Syntax.IsDigit( currentChar ) )
					{
						do
						{
							pos++;
						}
						while( pos < s.Length && Syntax.IsDigit( currentChar ) );
					}
					else
					{
						throw new KFFParseException( "Expected to find [0-9], but found '" + currentChar + "' (" + LineData.Calculate( this.s, this.pos ) + ")." );
					}
				}
				if( type == DataType.Integer )
				{
					CheckValueInteger( payloadStart );
				}
				if( type == DataType.Decimal )
				{
					CheckValueDecimal( payloadStart );
				}
				// Sprawdza, czy wartość można wepchać do danego typu.
				
				return;
			}
			throw new KFFParseException( "Invalid char. '" + currentChar + "' (" + LineData.Calculate( this.s, this.pos ) + ")." );
		}

		private void ValidateTag( ref List<string> encounteredNames )
		{
			string name;
			DataType __unused__;
			ValidateName( out name );

			if( encounteredNames.Contains( name ) )
			{
				throw new KFFParseException( "Repeated Tag's name '" + name + "' (" + LineData.Calculate( this.s, this.pos ) + ")." );
			}
			encounteredNames.Add( name );

			SkipWhiteSpaces();

			// '='

			if( currentChar == Syntax.NAME_PAYLOAD_SEPARATOR )
			{
				pos++;
			}
			else
			{
				throw new KFFParseException( "Expected to find '" + Syntax.NAME_PAYLOAD_SEPARATOR + "', but found '" + currentChar + "' (" + LineData.Calculate( this.s, this.pos ) + ")." );
			}

			SkipWhiteSpaces();

			ValidatePayload( out __unused__ );

			SkipWhiteSpaces();

			// ';'
			//Debug.Log( "Checking tag: " + name );
			if( currentChar == Syntax.TAG_END )
			{
				// Zapewnij, że głowica nie wyjedzie poza tablicę, jeśli trafi na koniec ostatniego TAG'u.
				if( pos < s.Length - 1 )
				{
					pos++;
				}
			}
			else
			{
				throw new KFFParseException( "Expected to find '" + Syntax.TAG_END + "', but found '" + currentChar + "' (" + LineData.Calculate( this.s, this.pos ) + ")." );
			}
		}

		// Class is a collection of tags.
		private void ValidateClass()
		{
			List<string> names = new List<string>();
			while( true )
			{
				SkipWhiteSpaces();
				// a jak nie ma drugiego tagu
				if( pos >= s.Length - 1 || currentChar == Syntax.CLASS_CLOSING )
				{
					return;
				}
				ValidateTag( ref names );
			}
		}

		// List is a collection of payloads, separated by ','.
		private void ValidateList()
		{
			DataType? listsDataType = null;
			DataType payloadsDataType;

			SkipWhiteSpaces();

			// a jak nie ma tagów wewnątrz.
			if( pos >= s.Length - 1 || currentChar == Syntax.LIST_CLOSING )
			{
				return;
			}

		ValidateList_AnotherTag:

			ValidatePayload( out payloadsDataType );
			if( listsDataType.HasValue )
			{
				if( listsDataType != payloadsDataType )
				{
					throw new KFFParseException( "Mismatched data types inside of the list." );
				}
			}
			else
			{
				listsDataType = payloadsDataType;
			}

			SkipWhiteSpaces();

			if( currentChar == Syntax.LIST_ELEMENT_SEPARATOR )
			{
				pos++;
				SkipWhiteSpaces();
				goto ValidateList_AnotherTag;
			}
			else
			{
				SkipWhiteSpaces();

				if( currentChar == Syntax.LIST_CLOSING )
				{
					return;
				}
				else
				{
					throw new KFFParseException( "Expected to find '" + Syntax.LIST_CLOSING + "', but found '" + currentChar + "' (" + LineData.Calculate( this.s, this.pos ) + ")." );
				}
			}
		}

		// Checks if the string between payloadStart and pos can be represented by an Integer data type.
		private void CheckValueInteger( int payloadStart )
		{
			string text = s.Substring( payloadStart, pos - payloadStart );
			if( !long.TryParse( text, System.Globalization.NumberStyles.Number, Syntax.numberFormat, out long o ) )
			{
				throw new KFFParseException( "The value '" + text + "' can't be represented by a 'long' (Integer) datatype (" + LineData.Calculate( this.s, this.pos ) + ")." );
			}
		}

		// Checks if the string between payloadStart and pos can be represented by a Decimal data type.
		private void CheckValueDecimal( int payloadStart )
		{
			string text = s.Substring( payloadStart, pos - payloadStart );
			if( !double.TryParse( text, System.Globalization.NumberStyles.Number, Syntax.numberFormat, out double o ) )
			{
				throw new KFFParseException( "The value '" + text + "' can't be represented by a 'double' (Decimal) datatype (" + LineData.Calculate( this.s, this.pos ) + ")." );
			}
		}



		/// <summary>
		/// Runs the validation process. Any errors will result in exceptions being thrown.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown when the string to validate is null.</exception>
		/// <exception cref="KFFParseException">Thrown when the string is not a valid KFF representation.</exception>
		public void Validate()
		{
			if( s == null )
			{
				throw new ArgumentNullException( "The string to validate is null." );
			}

			if( s == "" )
			{
				return;
			}
			
			List<string> names = new List<string>();
			while( true )
			{
				SkipWhiteSpaces();
				// a jak nie ma drugiego tagu
				if( pos >= s.Length - 1 )
				{
					return;
				}
				ValidateTag( ref names );
			}
		}

		private void SkipWhiteSpaces()
		{
			while( this.pos < this.s.Length && Syntax.IsWhiteSpace( currentChar ) )
			{
				this.pos++;
			}
		}

		/// <summary>
		/// Validates a tag's name.
		/// </summary>
		/// <param name="name">The string containing only the name.</param>
		/// <exception cref="ArgumentNullException">Thrown when the string is null or empty.</exception>
		/// <exception cref="KFFParseException">Thrown when the string isn't a valid tag's name.</exception>
		public static void ValidateName( string name )
		{
			if( string.IsNullOrEmpty( name ) )
			{
				throw new ArgumentNullException( "The name can't be null or empty." );
			}
			if( !(Syntax.IsAlphabetical( name[0] ) || name[0] == '_') )
			{
				throw new KFFParseException( "Expected to find [A-Za-z_], but found '" + name[0] + "' (" + LineData.Calculate( name, 0 ) + ")." );
			}
			for( int i = 1; i < name.Length; i++ )
			{
				if( !(Syntax.IsAlphaNumerical( name[i] ) || name[i] == '_') )
				{
					throw new KFFParseException( "Expected to find [A-Za-z0-9_], but found '" + name[i] + "' (" + LineData.Calculate( name, i ) + ")." );
				}
			}
		}
	}
}