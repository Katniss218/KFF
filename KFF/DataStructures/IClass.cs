﻿namespace KFF.DataStructures
{
	/// <summary>
	/// All class types (types that hold tags as their payload), in KFF, implement this interface.
	/// </summary>
	public interface IClass
	{
		int count { get; }

		bool Has( string name );

		bool TryGet( string name, out Tag output );

		Tag Get( string name );
		Tag[] GetAll();

		void Set( params Tag[] tags );

		void Remove( string name );

		void Clear();
	}
}