namespace KFF
{
	/// <summary>
	/// Implement this if you want to create an object that can be serialized into KFF.
	/// </summary>
	public interface IKFFSerializable
	{
		/// <summary>
		/// Called when the KFFWriter serializes the object. Every object is serialized into it's own, separate class.
		/// </summary>
		/// <param name="serializer">The class that the object will be serialized into.</param>
		/// the obj's scope is set to the class that you are writing to.
		void SerializeKFF( KFFSerializer serializer );

		/// <summary>
		/// Called when the KFFReader deserializes the object. Every object is serialized into it's own, separate class.
		/// </summary>
		/// <param name="serializer">The class thet the object will be separated into.</param>
		/// the obj's scope is set to the class that you are reading from.
		void DeserializeKFF( KFFSerializer serializer );
	}
}