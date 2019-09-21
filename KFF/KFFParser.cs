using KFF.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace KFF
{
	/// <summary>
	/// Parses a string into KFF file and vice versa.
	/// </summary>
	public sealed class KFFParser
	{
		private string fileName;
		private string s;
		private int pos;
		private char currentChar
		{
			get
			{
				if( this.pos >= s.Length )
				{
					throw new KFFEndOfFileException( "Unexpected end of file." );
				}
				return s[pos];
			}
		}

		private bool SilentPeek( string comparisonToken )
		{
			if( this.pos + comparisonToken.Length - 1 >= this.s.Length )
			{
				return false;
			}
			return this.s.Substring( this.pos, comparisonToken.Length ) == comparisonToken;
		}
		
		/// <summary>
		/// Should the writer add newlines around the Name/Payload Separator?
		/// </summary>
		public BeforeAfterMask namePayloadSeparatorNL { get; set; }

		/// <summary>
		/// Should the writer add a space around the Name/Payload Separator?
		/// </summary>
		public BeforeAfterMask namePayloadSeparatorSPC { get; set; }

		/// <summary>
		/// Should the writer add newlines around the Tag End?
		/// </summary>
		public BeforeAfterMask tagEndNL { get; set; }

		/// <summary>
		/// Should the writer add newlines around the Tag End?
		/// </summary>
		public BeforeAfterMask tagListElementSeparatorNL { get; set; }

		/// <summary>
		/// Should the writer add newlines around Class Opening?
		/// </summary>
		public BeforeAfterMask classOpeningNL { get; set; }

		/// <summary>
		/// Should the writer add newlines around Class Closing?
		/// </summary>
		public BeforeAfterMask classClosingNL { get; set; }

		/// <summary>
		/// Should the writer add newlines around List Opening?
		/// </summary>
		public BeforeAfterMask listOpeningNL { get; set; }

		/// <summary>
		/// Should the writer add newlines around List Closing?
		/// </summary>
		public BeforeAfterMask listClosingNL { get; set; }

		/// <summary>
		/// Should the writer add a TAB char for every tag/payload inside of a Class datatype.
		/// </summary>
		public bool tabInsideClass { get; set; }

		/// <summary>
		/// Should the writer add a TAB char for every tag/payload inside of a List datatype.
		/// </summary>
		public bool tabInsideList { get; set; }



		private void SkipWhiteSpacesAndComments()
		{
			while( this.pos < this.s.Length && (Syntax.IsWhiteSpace( currentChar ) || currentChar == Syntax.COMMENT[0]) )
			{
				if( SilentPeek( Syntax.COMMENT ) )
				{
					Comment();
					continue;
				}
				if( Syntax.IsWhiteSpace( currentChar ) )
				{
					this.pos++;
					continue;
				}
				throw new KFFParseException( "Invalid token '" + currentChar + "' (" + TextFileData.Calculate( this.fileName, this.s, this.pos ) + ")." );
			}
		}

		private void Comment()
		{
			this.pos += Syntax.COMMENT.Length;

			string lineEnding = Environment.NewLine;
			do
			{
				this.pos++;
			}
			while( this.pos + lineEnding.Length < this.s.Length && s.Substring( this.pos, lineEnding.Length ) != lineEnding );

			this.pos += lineEnding.Length;
		}
		
		private string Name()
		{
			if( Syntax.IsAlphabetical( currentChar ) || currentChar == '_' ) // Nazwy TAG'ów zaczynają się literą lub '_'.
			{
				StringBuilder sb = new StringBuilder();

				do
				{
					sb.Append( currentChar );
					pos++;

				} while( Syntax.IsAlphaNumerical( currentChar ) || currentChar == '_' );
				
				return sb.ToString();
			}
			throw new KFFParseException( "Expected to find [A-Za-z_], but found '" + currentChar + "' (" + TextFileData.Calculate( this.fileName, this.s, this.pos ) + ")." );
		}

		private void Digits( ref StringBuilder sb )
		{
			do
			{
				sb.Append( currentChar );
				pos++;

			} while( Syntax.IsDigit( currentChar ) );
		}

		private PayloadBoolean True()
		{
			pos++;
			for( int i = 1; i < Syntax.TOKEN_TRUE.Length; pos++, i++ )
			{
				// If any of the chars is not valid, throw an exception.
				if( currentChar != Syntax.TOKEN_TRUE[i] )
				{
					throw new KFFParseException( "Expected to find '" + Syntax.TOKEN_TRUE[i] + "', but found '" + currentChar + "' (" + TextFileData.Calculate( this.fileName, this.s, this.pos ) + ")." );
				}
			}

			SkipWhiteSpacesAndComments();
			// If no exception was thrown, that means there is a 'true' boolean value.
			return new PayloadBoolean( true );
		}

		private PayloadBoolean False()
		{
			pos++;
			for( int i = 1; i < Syntax.TOKEN_FALSE.Length; pos++, i++ )
			{
				// If any of the chars is not valid, throw an exception.
				if( currentChar != Syntax.TOKEN_FALSE[i] )
				{
					throw new KFFParseException( "Expected to find '" + Syntax.TOKEN_FALSE[i] + "', but found '" + currentChar + "' (" + TextFileData.Calculate( this.fileName, this.s, this.pos ) + ")." );
				}
			}

			SkipWhiteSpacesAndComments();
			// If no exception was thrown, that means there is a 'true' boolean value.
			return new PayloadBoolean( false );
		}

		private PayloadString PayloadString()
		{
			pos++; // '"'

			StringBuilder sb = new StringBuilder();
			do
			{
				// If right after the '"' is another '"' - we got an empty string payload.
				if( currentChar == Syntax.STRING_CONTAINER )
				{
					pos++;

					SkipWhiteSpacesAndComments();

					return new PayloadString( "" );
				}
				// If the char is escaping something...
				if( currentChar == Syntax.ESCAPE_CHAR )
				{
					pos++; // '\'

					// If it is escaping something that can be escaped...
					// Append the real char which that sequence is escaping.
					if( Syntax.IsEscapable( currentChar ) )// currentChar == KFFSyntax.STRING_CONTAINER )
					{
						sb.Append( Syntax.GetUnescaped( s.Substring( pos - 1, 2 ) ) );
						pos++;
					}
					// If it is escaping an unescapable char...
					else
					{
						throw new KFFParseException( "The char. '" + currentChar + "' isn't escapable (" + TextFileData.Calculate( this.fileName, this.s, this.pos ) + ")." );
					}
				}
				// If the char is NOT escaping anything...
				else
				{
					sb.Append( currentChar );
					pos++;
				}

			} while( currentChar != Syntax.STRING_CONTAINER );

			pos++; // '"'

			SkipWhiteSpacesAndComments();

			return new PayloadString( sb.ToString() );
		}

		private PayloadClass PayloadClass()
		{
			PayloadClass payload = new PayloadClass();

			pos++; // '{'

			SkipWhiteSpacesAndComments(); // to ustawi na pierwszym tagu, a każdy kolejny będzie z automatu bo tagi skipują WS'y za sobą.

			while( currentChar != Syntax.CLASS_CLOSING )
			{
				int begin = pos;
				Tag newTag = Tag();
				if( payload.Has( newTag.name ) )
				{
					throw new KFFParseException( "Found duplicated Tag '" + newTag.name + "' (" + TextFileData.Calculate( this.fileName, this.s, begin ) + ")." );
				}
				payload.Set( newTag );
			}

			pos++; // '}'

			SkipWhiteSpacesAndComments();

			return payload;
		}

		private PayloadList PayloadList()
		{
			PayloadList payload = new PayloadList();

			pos++; // '['

			SkipWhiteSpacesAndComments();

			// If there are no payloads inside of the list, return an empty list.
			if( currentChar == Syntax.LIST_CLOSING )
			{
				pos++; // ']'

				SkipWhiteSpacesAndComments();

				return payload;
			}


		funcPayload_AnotherPayload:

			Payload p = Payload();

			// If the list has a type assigned and the payload is not of this type.
			if( payload.listType != DataType.EmptyList && p.type != payload.listType )
			{
				throw new KFFParseException( "Found mismatched payload '" + p.type.ToString() + "' (" + TextFileData.Calculate( this.fileName, this.s, this.pos ) + ")." );
			}
			payload.Add( p );

			if( currentChar == Syntax.LIST_ELEMENT_SEPARATOR )
			{
				pos++; // ','

				SkipWhiteSpacesAndComments();

				goto funcPayload_AnotherPayload;
			}

			if( currentChar == Syntax.LIST_CLOSING )
			{
				pos++; // ']'

				SkipWhiteSpacesAndComments();

				return payload;
			}

			throw new KFFParseException( "Expected to find '" + Syntax.LIST_ELEMENT_SEPARATOR + "' or '" + Syntax.LIST_CLOSING + "', but found '" + currentChar + "' (" + TextFileData.Calculate( this.fileName, this.s, pos ) + ")." );
		}

		private PayloadDecimal NumberNaN()
		{
			pos++;
			for( int i = 1; i < Syntax.TOKEN_NOT_A_NUMBER.Length; pos++, i++ )
			{
				// If any of the chars is not valid, throw an exception.
				if( currentChar != Syntax.TOKEN_NOT_A_NUMBER[i] )
				{
					throw new KFFParseException( "Expected to find '" + Syntax.TOKEN_NOT_A_NUMBER[i] + "', but found '" + currentChar + "' (" + TextFileData.Calculate( this.fileName, this.s, this.pos ) + ")." );
				}
			}

			SkipWhiteSpacesAndComments();

			return new PayloadDecimal( double.NaN );
		}

		private PayloadDecimal NumberInfinityPosNeg( bool isNegative )
		{
			// This function assumes that the pos is at the 1st character of the "Infinity" keyword (minus needs to be skipped earlier).

			pos++;
			for( int i = 1; i < Syntax.TOKEN_INFINITY.Length; pos++, i++ )
			{
				// If any of the chars is not valid, throw an exception.
				if( currentChar != Syntax.TOKEN_INFINITY[i] )
				{
					throw new KFFParseException( "Expected to find '" + Syntax.TOKEN_INFINITY[i] + "', but found '" + currentChar + "' (" + TextFileData.Calculate( this.fileName, this.s, this.pos ) + ")." );
				}
			}

			SkipWhiteSpacesAndComments();

			if( isNegative )
			{
				return new PayloadDecimal( double.NegativeInfinity );
			}
			return new PayloadDecimal( double.PositiveInfinity );
		}

		private Payload NumericLiteral( bool isNegative )
		{
			// This function assumes that the pos is at the 1st character of a numeric literal.

			StringBuilder sb = new StringBuilder();

			// If has encountered negative sign, append it as it's a part of the number.
			if( isNegative )
			{
				sb.Append( Syntax.NEGATIVE_SIGN );
			}

			Digits( ref sb );

			bool isDecimal = false;

			//	Decimal
			//
			if( currentChar == Syntax.DECIMAL_SEPARATOR )
			{
				isDecimal = true;

				sb.Append( currentChar );   // '.'
				pos++;                      // '.'

				if( Syntax.IsDigit( currentChar ) )
				{
					Digits( ref sb );
				}
				else
				{
					throw new KFFParseException( "Expected to find [0-9], but found '" + currentChar + "' (" + TextFileData.Calculate( this.fileName, this.s, this.pos ) + ")." );
				}
			}

			//	Decimal (exponential)
			//
			if( currentChar == 'e' || currentChar == 'E' )
			{
				isDecimal = true;

				sb.Append( currentChar );

				pos++;

				if( currentChar == Syntax.NEGATIVE_SIGN )
				{
					sb.Append( currentChar );

					pos++;
				}
				if( Syntax.IsDigit( currentChar ) )
				{
					do
					{
						sb.Append( currentChar );
						pos++;

					} while( Syntax.IsDigit( currentChar ) );
				}
				else
				{
					throw new KFFParseException( "Expected to find [0-9], but found '" + currentChar + "' (" + TextFileData.Calculate( this.fileName, this.s, this.pos ) + ")." );
				}
			}

			SkipWhiteSpacesAndComments(); // right before the ; char

			//	Decimal
			//
			if( isDecimal )
			{
				return TryParseDecimal( sb.ToString() );
			}

			//	Integer
			//
			else
			{
				return TryParseInteger( sb.ToString() );
			}
		}

		
		private Payload Payload()
		{
			// Start at the beginning char, whitespaces need to be skipped earlier.

			//
			//	CLASS
			//
			if( currentChar == Syntax.CLASS_OPENING )
			{
				return PayloadClass();
			}

			//
			//	LIST
			//
			if( currentChar == Syntax.LIST_OPENING )
			{
				return PayloadList();
			}

			//
			//	STRING
			//
			if( currentChar == Syntax.STRING_CONTAINER )
			{
				return PayloadString();
			}

			//
			//	BOOLEAN
			//
			if( currentChar == Syntax.TOKEN_TRUE[0] )
			{
				return True();
			}
			if( currentChar == Syntax.TOKEN_FALSE[0] )
			{
				return False();
			}

			//
			//	NUMBER (Integer / Decimal)
			//
			// NaN
			if( currentChar == Syntax.TOKEN_NOT_A_NUMBER[0] )
			{
				return NumberNaN();
			}
			bool negativeSignFlag = false;
			if( currentChar == Syntax.NEGATIVE_SIGN )
			{
				negativeSignFlag = true;

				pos++; // '-'
			}
			if( currentChar == Syntax.TOKEN_INFINITY[0] )
			{
				return NumberInfinityPosNeg( negativeSignFlag );
			}
			if( Syntax.IsDigit( currentChar ) ) // tagi numeryczne zaczynają się liczbą lub minusem w wypadku ujemnego tagu.
			{
				return NumericLiteral( negativeSignFlag );
			}
			else
			{
				// If the parser found non-digits after the '-' (negative sign).
				// Throw an exception.
				if( negativeSignFlag )
				{
					throw new KFFParseException( "Expected to find [0-9], but found '" + currentChar + "' (" + TextFileData.Calculate( this.fileName, this.s, this.pos ) + ")." );
				}
			}
			
			throw new KFFParseException( "Unexpected char. '" + currentChar + "' (" + TextFileData.Calculate( this.fileName, this.s, this.pos ) + ")." );
		}

		private Tag Tag()
		{
			// we start directly at the name.
			string name = Name();

			this.SkipWhiteSpacesAndComments();

			if( this.currentChar != Syntax.NAME_PAYLOAD_SEPARATOR )
			{
				throw new KFFParseException( "Expected to find '" + Syntax.NAME_PAYLOAD_SEPARATOR + "', but found '" + currentChar + "' (" + TextFileData.Calculate( this.fileName, this.s, this.pos ) + ")." );
			}
			pos++; // '='

			this.SkipWhiteSpacesAndComments();

			Payload payload = Payload();

			this.SkipWhiteSpacesAndComments();

			if( this.currentChar != Syntax.TAG_END )
			{
				throw new KFFParseException( "Expected to find '" + Syntax.TAG_END + "', but found '" + currentChar + "' (" + TextFileData.Calculate( this.fileName, this.s, this.pos ) + ")." );
			}
			pos++; // ';'

			this.SkipWhiteSpacesAndComments();

			if( payload.type == DataType.Boolean )
			{
				return new TagBoolean( name, (PayloadBoolean)payload );
			}
			if( payload.type == DataType.Integer )
			{
				return new TagInteger( name, (PayloadInteger)payload );
			}
			if( payload.type == DataType.Decimal )
			{
				return new TagDecimal( name, (PayloadDecimal)payload );
			}
			if( payload.type == DataType.String )
			{
				return new TagString( name, (PayloadString)payload );
			}
			if( payload.type == DataType.Class )
			{
				return new TagClass( name, (PayloadClass)payload );
			}
			if( payload.type == DataType.List )
			{
				return new TagList( name, (PayloadList)payload );
			}
			throw new KFFParseException("Invalid payload type '" + payload.type.ToString() + "' (" + TextFileData.Calculate( this.fileName, this.s, this.pos ) + ")." );
		}

		private PayloadInteger TryParseInteger( string numberText )
		{
			if( !long.TryParse( numberText, Syntax.numberStyle, Syntax.numberFormat, out long num ) )
			{
				throw new KFFParseException( "The value '" + numberText + "' can't be represented by a Integer datatype ('long') (" + TextFileData.Calculate( this.fileName, this.s, this.pos ) + ")." );
			}
			return new PayloadInteger( num );
		}

		private PayloadDecimal TryParseDecimal( string numberText )
		{
			if( !double.TryParse( numberText, Syntax.numberStyle, Syntax.numberFormat, out double num ) )
			{
				throw new KFFParseException( "The value '" + numberText + "' can't be represented by a Decimal datatype ('double') (" + TextFileData.Calculate( this.fileName, this.s, this.pos ) + ")." );
			}
			return new PayloadDecimal( num );
		}

		/// <summary>
		/// Parses a string representation into a KFF file. Throws an exception if the string is malformed.
		/// </summary>
		/// <param name="fileName">The name of the file that will be parsed.</param>
		/// <param name="s">The string to parse.</param>
		public KFFFile Parse( string fileName, string s )
		{
			if( s == null )
			{
				throw new ArgumentNullException( "The string to parse can't be null." );
			}
			if( s == "" )
			{
				return new KFFFile( fileName );
			}

			this.fileName = fileName;
			this.s = s;
			this.pos = 0;

			KFFFile f = new KFFFile( fileName );
			
			SkipWhiteSpacesAndComments();

			while( pos < s.Length )
			{
				int begin = pos;
				Tag newTag = Tag();
				if( f.Has( newTag.name ) )
				{
					throw new KFFParseException( "Found duplicated Tag '" + newTag.name + "' (" + TextFileData.Calculate( this.fileName, this.s, begin ) + ")." );
				}
				f.Set( newTag );
			}
			return f;
		}





		private string StringFromPayloadBoolean( PayloadBoolean t )
		{
			return t ? Syntax.TOKEN_TRUE : Syntax.TOKEN_FALSE;
		}

		private string StringFromPayloadInteger( PayloadInteger t )
		{
			return ((long)t).ToString( Syntax.numberFormat );
		}

		private string StringFromPayloadDecimal( PayloadDecimal t )
		{
			double num = (double)t;
			if( double.IsNaN( num ) )
			{
				return Syntax.TOKEN_NOT_A_NUMBER;
			}
			if( double.IsPositiveInfinity( num ) )
			{
				return Syntax.TOKEN_INFINITY;
			}
			if( double.IsNegativeInfinity( num ) )
			{
				return Syntax.NEGATIVE_SIGN + Syntax.TOKEN_INFINITY;
			}
			string s = num.ToString( "g17", Syntax.numberFormat );
			if( !s.Contains( "e" ) && !s.Contains( "E" ) )
			{
				if( !MathHelper.HasDecimalDigits( t ) )
				{
					s += ".0";
				}
			}
			return s;
		}

		private string StringFromPayloadString( PayloadString t )
		{
			StringBuilder sb = new StringBuilder();
			string val = t.value;
			sb.Append( Syntax.STRING_CONTAINER );
			for( int i = 0; i < val.Length; i++ )
			{
				// If it is escapable, append the escaped sequence.
				if( Syntax.IsEscapable( val[i] ) )
				{
					sb.Append( Syntax.GetEscaped( val[i] ) );
				}
				// Otherwise, append just the char.
				else
				{
					sb.Append( val[i] );
				}
			}
			sb.Append( Syntax.STRING_CONTAINER );
			return sb.ToString();
		}

		private string StringFromPayloadClass( PayloadClass t )
		{
			StringBuilder sb = new StringBuilder();

			if( ((int)classOpeningNL & 1) == 1 ) // before
				sb.Append( Environment.NewLine );
			sb.Append( Syntax.CLASS_OPENING );

			if( ((int)classOpeningNL & 2) == 2 ) // after
				sb.Append( Environment.NewLine );

			if( tabInsideClass )
			{
				// insert tab at the front of the class, right before the first tag.
				sb.Append( "\t" );
			}
			for( int i = 0; i < t.value.Count; i++ )
			{
				string tag = StringFromAnyTag( t.value[i] );

				if( tabInsideClass )
				{
					// insert tab after every newline char (skips the front of the tag, as there's no newline char there).
					tag = tag.Replace( Environment.NewLine, Environment.NewLine + "\t" );
				}
				sb.Append( tag );
			}


			if( ((int)classClosingNL & 1) == 1 ) // before
				sb.Append( Environment.NewLine );

			sb.Append( Syntax.CLASS_CLOSING );

			if( ((int)classClosingNL & 2) == 2 ) // after
				sb.Append( Environment.NewLine );

			return sb.ToString();
		}

		private string StringFromPayloadList( PayloadList t )
		{
			StringBuilder sb = new StringBuilder();

			if( ((int)listOpeningNL & 1) == 1 ) // before
				sb.Append( Environment.NewLine );

			sb.Append( Syntax.LIST_OPENING );

			if( ((int)listOpeningNL & 2) == 2 ) // after
				sb.Append( Environment.NewLine );


			for( int i = 0; i < t.count; i++ )
			{
				DataType listType = t.listType;
				string payload = "";

				if( listType == DataType.Boolean )
					payload = StringFromPayloadBoolean( (PayloadBoolean)t.Get( i ) );

				if( listType == DataType.Integer )
					payload = StringFromPayloadInteger( (PayloadInteger)t.Get( i ) );
				if( listType == DataType.Decimal )
					payload = StringFromPayloadDecimal( (PayloadDecimal)t.Get( i ) );

				if( listType == DataType.String )
					payload = StringFromPayloadString( (PayloadString)t.Get( i ) );

				if( listType == DataType.Class )
					payload = StringFromPayloadClass( (PayloadClass)t.Get( i ) );
				if( listType == DataType.List )
					payload = StringFromPayloadList( (PayloadList)t.Get( i ) );

				if( tabInsideList )
				{
					// insert tab at the front, right before the first payload in the list.
					sb.Append( "\t" );
					// insert tab after every newline char (skips the front of the first payload, as there's no newline char before it).
					payload = payload.Replace( Environment.NewLine, Environment.NewLine + "\t" );
				}

				sb.Append( payload );

				if( i != t.count - 1 )
				{
					if( ((int)tagListElementSeparatorNL & 1) == 1 ) // before
						sb.Append( Environment.NewLine );

					sb.Append( Syntax.LIST_ELEMENT_SEPARATOR );

					if( ((int)tagListElementSeparatorNL & 2) == 2 ) // after
						sb.Append( Environment.NewLine );
				}
			}


			if( ((int)listClosingNL & 1) == 1 ) // before
				sb.Append( Environment.NewLine );

			sb.Append( Syntax.LIST_CLOSING );

			if( ((int)listClosingNL & 2) == 2 ) // after
				sb.Append( Environment.NewLine );

			return sb.ToString();
		}

		private string StringFromAnyTag( Tag t )
		{
			StringBuilder sb = new StringBuilder();

			// Name

			sb.Append( t.name );

			// N/P Sep

			if( ((int)namePayloadSeparatorNL & 1) == 1 ) // before
				sb.Append( Environment.NewLine );

			if( ((int)namePayloadSeparatorSPC & 1) == 1 ) // before
				sb.Append( " " );

			sb.Append( Syntax.NAME_PAYLOAD_SEPARATOR );

			if( ((int)namePayloadSeparatorNL & 2) == 2 ) // after
				sb.Append( Environment.NewLine );

			if( ((int)namePayloadSeparatorSPC & 2) == 2 ) // before
				sb.Append( " " );

			// Payload

			if( t.type == DataType.Boolean )
			{
				sb.Append( this.StringFromPayloadBoolean( ((TagBoolean)t).payload ) );
			}

			else if( t.type == DataType.Integer )
			{
				sb.Append( this.StringFromPayloadInteger( ((TagInteger)t).payload ) );
			}
			else if( t.type == DataType.Decimal )
			{
				sb.Append( this.StringFromPayloadDecimal( ((TagDecimal)t).payload ) );
			}

			else if( t.type == DataType.String )
			{
				sb.Append( this.StringFromPayloadString( ((TagString)t).payload ) );
			}

			else if( t.type == DataType.Class )
			{
				sb.Append( this.StringFromPayloadClass( ((TagClass)t).payload ) );
			}
			else if( t.type == DataType.List )
			{
				sb.Append( this.StringFromPayloadList( ((TagList)t).payload ) );
			}
			else
			{
				throw new KFFException( "Unknown Tag type '" + t.GetType().ToString() + "'!" );
			}

			// Tag End

			if( ((int)tagEndNL & 1) == 1 ) // before
				sb.Append( Environment.NewLine );

			sb.Append( Syntax.TAG_END );

			if( ((int)tagEndNL & 2) == 2 ) // after
				sb.Append( Environment.NewLine );

			return sb.ToString();
		}

		/// <summary>
		/// Gets the string representation of the file in the writer.
		/// </summary>
		public string ToString( KFFFile file )
		{
			StringBuilder sb = new StringBuilder();
			List<Tag> tags = file.tags.value;
			
			for( int i = 0; i < tags.Count; i++ )
			{
				string tag = StringFromAnyTag( tags[i] );
				sb.Append( tag );
			}

			return sb.ToString();
		}
	}
}
