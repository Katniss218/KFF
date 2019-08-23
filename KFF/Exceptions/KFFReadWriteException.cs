using System;

namespace KFF
{
	/// <summary>
	/// Represents an error in the process of validation.
	/// </summary>
	public class KFFReadWriteException : KFFException
	{
		internal KFFReadWriteException() : base()
		{

		}

		internal KFFReadWriteException( string message ) : base( message )
		{

		}

		internal KFFReadWriteException( string message, Exception innerException ) : base( message, innerException )
		{

		}
	}
}