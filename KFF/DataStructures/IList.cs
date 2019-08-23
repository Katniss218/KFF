
namespace KFF.DataStructures
{
	/// <summary>
	/// All list types (types that hold payloads of single type as their payload), in KFF, implement this interface.
	/// </summary>
	public interface IList
	{
		//  - method for writing serializables (so you can recursively write them from IKFFSerializable).

		int count { get; }
		
		DataType listType { get; }
		
		bool Has( int index );
		
		bool TryGet( int index, out Payload output );

		Payload Get( int index );
		Payload[] GetAll();
		
		void Add( params Payload[] payload );
		
		void Remove( int index );
		
		void Clear();
	}
}