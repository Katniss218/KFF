using System;

namespace KFF
{
	/// <summary>
	/// Represents an error during object serialization. Feel free to throw this in your IKFFSerializable objects when something bad happens.
	/// </summary>
	public class KFFSerializationException : KFFReadWriteException
	{
		/// <summary>
		/// The object throwing the exception.
		/// </summary>
		public IKFFSerializable objectThrowing;

		/// <summary>
		/// Creates a new serialization exception.
		/// </summary>
		/// <param name="objectThrowing">Should be set to the object that is throwing the exception.</param>
		public KFFSerializationException( IKFFSerializable objectThrowing ) : base()
		{
			this.objectThrowing = objectThrowing;
		}

		/// <summary>
		/// Creates a new serialization exception with the specified message.
		/// </summary>
		/// <param name="objectThrowing">Should be set to the object that is throwing the exception.</param>
		/// <param name="message">The message.</param>
		public KFFSerializationException( IKFFSerializable objectThrowing, string message ) : base( message )
		{
			this.objectThrowing = objectThrowing;
		}

		/// <summary>
		/// Creates a new serialization exception with the specified message and an inner exception.
		/// </summary>
		/// <param name="objectThrowing">Should be set to the object that is throwing the exception.</param>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public KFFSerializationException( IKFFSerializable objectThrowing, string message, Exception innerException ) : base( message, innerException )
		{
			this.objectThrowing = objectThrowing;
		}
	}
}