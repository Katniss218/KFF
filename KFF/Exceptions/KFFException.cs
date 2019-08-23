using System;

namespace KFF
{
	/// <summary>
	/// Represents any KFF exception.
	/// </summary>
	public class KFFException : Exception
	{
		internal KFFException() : base()
		{

		}

		internal KFFException( string message ) : base( message )
		{

		}

		internal KFFException( string message, Exception innerException ) : base( message, innerException )
		{

		}
	}
}