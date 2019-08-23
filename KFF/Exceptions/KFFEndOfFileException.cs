using System;

namespace KFF
{
	/// <summary>
	/// Represents an error during object serialization. Feel free to throw this in your IKFFSerializable objects.
	/// </summary>
	public class KFFEndOfFileException : KFFParseException
	{
		internal KFFEndOfFileException() : base()
		{

		}

		internal KFFEndOfFileException( string message ) : base( message )
		{

		}

		internal KFFEndOfFileException( string message, Exception innerException ) : base( message, innerException )
		{

		}
	}
}