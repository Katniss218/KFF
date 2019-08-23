using System;

namespace KFF
{
	/// <summary>
	/// Thrown when a tag or payload is present when it should not be, or is not present when it should be.
	/// </summary>
	public class KFFObjectPresenceException : KFFException
	{
		internal KFFObjectPresenceException() : base()
		{

		}

		internal KFFObjectPresenceException( string message ) : base( message )
		{

		}

		internal KFFObjectPresenceException( string message, Exception innerException ) : base( message, innerException )
		{

		}
	}
}