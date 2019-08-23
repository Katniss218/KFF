using System;

namespace KFF
{
	/// <summary>
	/// Represents an error in the syntax.
	/// </summary>
	public class KFFParseException : KFFException
	{
		internal KFFParseException() : base()
		{

		}

		internal KFFParseException( string message ) : base( message )
		{

		}

		internal KFFParseException( string message, Exception innerException ) : base( message, innerException )
		{

		}
	}
}