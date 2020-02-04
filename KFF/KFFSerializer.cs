using KFF.DataStructures;
using System;
using System.IO;
using System.Text;
using Object = KFF.DataStructures.Object;

namespace KFF
{
	/// <summary>
	/// Serializes and deserializes C# objects of various types into a KFF's representation of them.
	/// <para>
	/// This class is intended to be used by the end-user.
	/// </para>
	/// </summary>
	public sealed class KFFSerializer
	{
		/// <summary>
		/// Contains the results of the analysis operation.
		/// </summary>
		public sealed class AnalysisData
		{
			/// <summary>
			/// True if the analysis found a tag or payload at the specified path (Read Only).
			/// </summary>
			public bool isSuccess { get; private set; }
			/// <summary>
			/// Checks if the analysis DIDN'T found any tags or payloads at the specified path (Read Only).
			/// </summary>
			public bool isFail { get { return !this.isSuccess; } }

			/// <summary>
			/// Contains the number of children (nested objects) of the analyzed object (Read Only).
			/// </summary>
			public int childCount { get; private set; }

			/// <summary>
			/// Is true if the current analyzed object is a tag, false otherwise (Read Only).
			/// </summary>
			public bool isTag { get; private set; }

			/// <summary>
			/// Is true if the current analyzed object is a payload, false otherwise (Read Only).
			/// </summary>
			public bool isPayload { get; private set; }

			internal AnalysisData( Object obj )
			{
				if( obj == null )
				{
					this.isSuccess = false;
					this.isTag = false;
					this.isPayload = false;
					this.childCount = 0;
				}
				else
				{
					this.isSuccess = true;
					this.isTag = obj.isTag;
					this.isPayload = obj.isPayload;

					if( obj.type == DataType.Class )
					{
						this.childCount = ((IClass)obj).count;
					}
					else if( obj.type == DataType.List )
					{
						this.childCount = ((IList)obj).count;
					}
					else
					{
						this.childCount = 0;
					}
				}
			}
		}
		/// <summary>
		/// Contains the root object of the current scope. Either a tag or payload. You shouldn't mess with this, unless you know what you are doing.
		/// <para>
		/// This can, BY DESIGN, be set outside of the file, to be able read/write from objects, that are not in the file.
		/// E.g.: To serialize into a Tag/Payload, that is not in the file (arbitrary Tag/Payload).
		/// </para>
		/// </summary>
		public Object scopeRoot { get; set; }

		/// <summary>
		/// Contains the file that the serializer operates on. The scope is reset to this file upon calling the 'ResetScope()' method (Read Only).
		/// </summary>
		public KFFFile file { get; private set; } // The file is ONLY used here, ...



		/// <summary>
		/// Creates a new KFF serializer and assigns a KFF file to it.
		/// </summary>
		/// <param name="file">The KFF file to operate on.</param>
		public KFFSerializer( KFFFile file )
		{
			this.file = file; // ... here, ...
			this.scopeRoot = file;
		}
		
#warning TODO! - make the KFFSerializer show the path & file name on exception.

		/// <summary>
		/// Wrapper method for creating a new KFFSerializer with the contents of a text file.
		/// </summary>
		/// <param name="path">The path to the file.</param>
		/// <param name="encoding">The encoding of the text file.</param>
		public static KFFSerializer ReadFromFile( string path, Encoding encoding )
		{
			string contents = null;
			try
			{
				contents = File.ReadAllText( path );
			}
			catch( Exception e )
			{
				throw new KFFException( "Error occurred when trying to open file '" + path + "'.", e );
			}

			KFFParser parser = new KFFParser();
			KFFFile kff = parser.Parse( path, contents );

			return new KFFSerializer( kff );
		}

		/// <summary>
		/// Wrapper method for writing the contents of this KFFSerializer (KFFSerializer.file) to a text file.
		/// </summary>
		/// <param name="path">The path to the file.</param>
		/// <param name="encoding">The encoding of the text file.</param>
		public void WriteToFile( string path, Encoding encoding )
		{
			KFFParser parser = new KFFParser();

			// TODO - parser settings.

			string kff = parser.ToString( this.file );
			try
			{
				File.WriteAllText( path, kff, encoding );
			}
			catch( Exception e )
			{
				throw new KFFException( "Error occurred when trying to save file '" + path + "'.", e );
			}
		}

		/// <summary>
		/// Resets the scope to the entire KFF file, and returns that file.
		/// </summary>
		public Object ResetScope() // no references...
		{
			this.scopeRoot = this.file; // ... and here.
			return this.scopeRoot;
		}

		/// <summary>
		/// Moves the scope to the object at the end of the specified path, and returns that object. If the <paramref name="keepItThere"/> is set to true, the scope will be set to the object at end of the path.
		/// <para>
		/// Moving scope works relative to the scopeRoot. So if you set it to an object outside of the file, you can freely read/write from another data tree.
		/// </para>
		/// </summary>
		/// <param name="path">The path along which to move the scope.</param>
		/// <param name="keepItThere">If set to true, the scope will be set to the object at end of the path.</param>
		public Object MoveScope( Path path, bool keepItThere )
		{
			// Throws an Exception, if the scope can't be moved all the way, to the end of the path.
			// This can be used to speed up searching, by removing unwanted objects from the search.

			// If the path is empty, don't move at all, just return the current scope.
			if( path.isEmpty )
			{
				return this.scopeRoot;
			}

			// The scope local to the function.
			Object localScopeRoot = this.scopeRoot;

			// If the path is not empty, move through all of the steps and return the result.
			for( int i = 0; i < path.nestLevel; i++ )
			{
				if( path[i].direction == PathDirection.Forward )
				{
					// Go forward (set the scope to the object 1 level deeper).
					// If the scopeRoot is a class, set the scopeRoot to the next element of the path, inside that class.
					if( localScopeRoot.type == DataType.Class )
					{
						if( path[i].destination == ObjectType.Payload )
						{
							throw new KFFReadWriteException( "Can't get a Payload inside of a class ('" + path[i].name + "')." );
						}
						localScopeRoot = ((IClass)localScopeRoot).Get( path[i].name );
						continue;
					}
					// If the scopeRoot is a list, set the scopeRoot to the next element of the path, inside that list.
					if( localScopeRoot.type == DataType.List )
					{
						if( path[i].destination == ObjectType.Tag )
						{
							throw new KFFReadWriteException( "Can't get a Tag inside of a list ('" + path[i].name + "')." );
						}
						localScopeRoot = ((IList)localScopeRoot).Get( path[i].index );
						continue;
					}
					// If the element is not a class not a list, and it is in the middle of the path (more elements nested deeper), throw an exception.
					if( i < path.nestLevel - 1 )
					{
						throw new KFFReadWriteException( "The file contains non-class/list element '" + path[i].name + "' in the middle of the path." );
					}
				}
				if( path[i].direction == PathDirection.Backward )
				{
					// Go backward (set the scope to the non-embedded object 1 level shallower).
					localScopeRoot = localScopeRoot.parent;
					if( localScopeRoot.isPayload )
					{
						// if the payload is a part of a tag or file, go back to the tag or file itself.
						if( ((Payload)localScopeRoot).isEmbedded )
						{
							localScopeRoot = localScopeRoot.parent;
						}
					}
					// if the scope root is payload which is a part of a tag, move back to that tag.
				}
			}
			if( keepItThere )
			{
				this.scopeRoot = localScopeRoot; // if the function doesn't local scope, move the "real" root to where the path points and leave it there.
			}
			return localScopeRoot;
		}



		//#################################################################################################################################################################

		//#################################################################################################################################################################
		//
		//				CHECKS
		//
		//#################################################################################################################################################################

		//#################################################################################################################################################################



		/// <summary>
		/// Performs an Analysis operation on the object at the end of the path. Returns true if a path is valid, and leads to a valid KFF object.
		/// <para>
		/// You can use this to obtain information about objects inside a KFF file.
		/// </para>
		/// </summary>
		/// <param name="path">The path to the object to analyze.</param>
		public AnalysisData Analyze( Path path )
		{
			try
			{
				Object analyzed = MoveScope( path, false );
				return new AnalysisData( analyzed );
			}
			catch( KFFException )
			{
				return new AnalysisData( null );
			}
		}




		//#################################################################################################################################################################

		//#################################################################################################################################################################
		//
		//				WRITING
		//
		//#################################################################################################################################################################

		//#################################################################################################################################################################



		// Writes a tag at the specified path.
		private void __WriteTag( Path path, bool keepItThere, Tag obj )
		{
			Object result = this.MoveScope( path, keepItThere );
			if( result.type == DataType.Class )
			{
				((IClass)result).Set( obj );
				return;
			}
			throw new KFFReadWriteException( "Can't set Tag '" + obj.name + "' to the non-class object '" + result.GetType().ToString() + "'." );
		}

		// Writes a payload at the specified path.
		private void __AppendPayload( Path path, bool keepItThere, Payload obj )
		{
			Object result = this.MoveScope( path, keepItThere );
			if( result.type == DataType.List )
			{
				((IList)result).Add( obj );
				return;
			}
			throw new KFFReadWriteException( "Can't add Payload '" + obj + "' to the non-list object '" + result.GetType().ToString() + "'." );
		}




		/// <summary>
		/// Writes a bool value at the specified path.
		/// </summary>
		/// <param name="path">The path to write the Tag at (points to the container).</param>
		/// <param name="name">The name of the new Tag.</param>
		/// <param name="value">The value to write.</param>
		public void WriteBool( Path path, string name, bool value ) // writes a tag named 'name' to the class at path 'path'.
		{
			this.__WriteTag( path, false, new TagBoolean( name, value ) );
		}

		/// <summary>
		/// Writes an array of bool values at the specified path.
		/// </summary>
		/// <param name="path">The path to write the TagList at (points to the container).</param>
		/// <param name="containerName">The name of the TagList containing the values.</param>
		/// <param name="values">The values to write.</param>
		public void WriteBoolArray( Path path, string containerName, bool[] values )
		{
			TagList container = new TagList( containerName );
			for( int i = 0; i < values.Length; i++ )
			{
				container.Add( new PayloadBoolean( values[i] ) );
			}
			this.__WriteTag( path, false, container );
		}

		/// <summary>
		/// Writes an sbyte value at the specified path.
		/// </summary>
		/// <param name="path">The path to write the Tag at (points to the container).</param>
		/// <param name="name">The name of the new Tag.</param>
		/// <param name="value">The value to write.</param>
		public void WriteSByte( Path path, string name, sbyte value ) // writes a tag named 'name' to the class at path 'path'.
		{
			__WriteTag( path, false, new TagInteger( name, value ) );
		}

		/// <summary>
		/// Writes an array of sbyte values at the specified path.
		/// </summary>
		/// <param name="path">The path to write the TagList at (points to the container).</param>
		/// <param name="containerName">The name of the TagList containing the values.</param>
		/// <param name="values">The values to write.</param>
		public void WriteSByteArray( Path path, string containerName, sbyte[] values )
		{
			TagList container = new TagList( containerName );
			for( int i = 0; i < values.Length; i++ )
			{
				container.Add( new PayloadInteger( values[i] ) );
			}
			this.__WriteTag( path, false, container );
		}

		/// <summary>
		/// Writes a byte value at the specified path.
		/// </summary>
		/// <param name="path">The path to write the Tag at (points to the container).</param>
		/// <param name="name">The name of the new Tag.</param>
		/// <param name="value">The value to write.</param>
		public void WriteByte( Path path, string name, byte value ) // writes a tag named 'name' to the class at path 'path'.
		{
			__WriteTag( path, false, new TagInteger( name, value ) );
		}

		/// <summary>
		/// Writes an array of byte values at the specified path.
		/// </summary>
		/// <param name="path">The path to write the TagList at (points to the container).</param>
		/// <param name="containerName">The name of the TagList containing the values.</param>
		/// <param name="values">The values to write.</param>
		public void WriteByteArray( Path path, string containerName, byte[] values )
		{
			TagList container = new TagList( containerName );
			for( int i = 0; i < values.Length; i++ )
			{
				container.Add( new PayloadInteger( values[i] ) );
			}
			this.__WriteTag( path, false, container );
		}

		/// <summary>
		/// Writes a short value at the specified path.
		/// </summary>
		/// <param name="path">The path to write the Tag at (points to the container).</param>
		/// <param name="name">The name of the new Tag.</param>
		/// <param name="value">The value to write.</param>
		public void WriteShort( Path path, string name, short value ) // writes a tag named 'name' to the class at path 'path'.
		{
			__WriteTag( path, false, new TagInteger( name, value ) );
		}

		/// <summary>
		/// Writes an array of short values at the specified path.
		/// </summary>
		/// <param name="path">The path to write the TagList at (points to the container).</param>
		/// <param name="containerName">The name of the TagList containing the values.</param>
		/// <param name="values">The values to write.</param>
		public void WriteShortArray( Path path, string containerName, short[] values )
		{
			TagList container = new TagList( containerName );
			for( int i = 0; i < values.Length; i++ )
			{
				container.Add( new PayloadInteger( values[i] ) );
			}
			this.__WriteTag( path, false, container );
		}

		/// <summary>
		/// Writes a ushort value at the specified path.
		/// </summary>
		/// <param name="path">The path to write the Tag at (points to the container).</param>
		/// <param name="name">The name of the new Tag.</param>
		/// <param name="value">The value to write.</param>
		public void WriteUShort( Path path, string name, ushort value ) // writes a tag named 'name' to the class at path 'path'.
		{
			__WriteTag( path, false, new TagInteger( name, value ) );
		}

		/// <summary>
		/// Writes an array of ushort values at the specified path.
		/// </summary>
		/// <param name="path">The path to write the TagList at (points to the container).</param>
		/// <param name="containerName">The name of the TagList containing the values.</param>
		/// <param name="values">The values to write.</param>
		public void WriteUShortArray( Path path, string containerName, ushort[] values )
		{
			TagList container = new TagList( containerName );
			for( int i = 0; i < values.Length; i++ )
			{
				container.Add( new PayloadInteger( values[i] ) );
			}
			this.__WriteTag( path, false, container );
		}

		/// <summary>
		/// Writes an int value at the specified path.
		/// </summary>
		/// <param name="path">The path to write the Tag at (points to the container).</param>
		/// <param name="name">The name of the new Tag.</param>
		/// <param name="value">The value to write.</param>
		public void WriteInt( Path path, string name, int value ) // writes a tag named 'name' to the class at path 'path'.
		{
			__WriteTag( path, false, new TagInteger( name, value ) );
		}

		/// <summary>
		/// Writes an array of int values at the specified path.
		/// </summary>
		/// <param name="path">The path to write the TagList at (points to the container).</param>
		/// <param name="containerName">The name of the TagList containing the values.</param>
		/// <param name="values">The values to write.</param>
		public void WriteIntArray( Path path, string containerName, int[] values )
		{
			TagList container = new TagList( containerName );
			for( int i = 0; i < values.Length; i++ )
			{
				container.Add( new PayloadInteger( values[i] ) );
			}
			this.__WriteTag( path, false, container );
		}

		/// <summary>
		/// Writes a uint value at the specified path.
		/// </summary>
		/// <param name="path">The path to write the Tag at (points to the container).</param>
		/// <param name="name">The name of the new Tag.</param>
		/// <param name="value">The value to write.</param>
		public void WriteUInt( Path path, string name, uint value ) // writes a tag named 'name' to the class at path 'path'.
		{
			__WriteTag( path, false, new TagInteger( name, value ) );
		}

		/// <summary>
		/// Writes an array of uint values at the specified path.
		/// </summary>
		/// <param name="path">The path to write the TagList at (points to the container).</param>
		/// <param name="containerName">The name of the TagList containing the values.</param>
		/// <param name="values">The values to write.</param>
		public void WriteUIntArray( Path path, string containerName, uint[] values )
		{
			TagList container = new TagList( containerName );
			for( int i = 0; i < values.Length; i++ )
			{
				container.Add( new PayloadInteger( values[i] ) );
			}
			this.__WriteTag( path, false, container );
		}

		/// <summary>
		/// Writes a long value at the specified path.
		/// </summary>
		/// <param name="path">The path to write the Tag at (points to the container).</param>
		/// <param name="name">The name of the new Tag.</param>
		/// <param name="value">The value to write.</param>
		public void WriteLong( Path path, string name, long value ) // writes a tag named 'name' to the class at path 'path'.
		{
			__WriteTag( path, false, new TagInteger( name, value ) );
		}

		/// <summary>
		/// Writes an array of long values at the specified path.
		/// </summary>
		/// <param name="path">The path to write the TagList at (points to the container).</param>
		/// <param name="containerName">The name of the TagList containing the values.</param>
		/// <param name="values">The values to write.</param>
		public void WriteLongArray( Path path, string containerName, long[] values )
		{
			TagList container = new TagList( containerName );
			for( int i = 0; i < values.Length; i++ )
			{
				container.Add( new PayloadInteger( values[i] ) );
			}
			this.__WriteTag( path, false, container );
		}

		/// <summary>
		/// Writes a float value at the specified path.
		/// </summary>
		/// <param name="path">The path to write the Tag at (points to the container).</param>
		/// <param name="name">The name of the new Tag.</param>
		/// <param name="value">The value to write.</param>
		public void WriteFloat( Path path, string name, float value ) // writes a tag named 'name' to the class at path 'path'.
		{
			this.__WriteTag( path, false, new TagDecimal( name, value ) );
		}

		/// <summary>
		/// Writes an array of float values at the specified path.
		/// </summary>
		/// <param name="path">The path to write the TagList at (points to the container).</param>
		/// <param name="containerName">The name of the TagList containing the values.</param>
		/// <param name="values">The values to write.</param>
		public void WriteFloatArray( Path path, string containerName, float[] values )
		{
			TagList container = new TagList( containerName );
			for( int i = 0; i < values.Length; i++ )
			{
				container.Add( new PayloadDecimal( values[i] ) );
			}
			this.__WriteTag( path, false, container );
		}

		/// <summary>
		/// Writes a double value at the specified path.
		/// </summary>
		/// <param name="path">The path to write the Tag at (points to the container).</param>
		/// <param name="name">The name of the new Tag.</param>
		/// <param name="value">The value to write.</param>
		public void WriteDouble( Path path, string name, double value ) // writes a tag named 'name' to the class at path 'path'.
		{
			this.__WriteTag( path, false, new TagDecimal( name, value ) );
		}

		/// <summary>
		/// Writes an array of double values at the specified path.
		/// </summary>
		/// <param name="path">The path to write the TagList at (points to the container).</param>
		/// <param name="containerName">The name of the TagList containing the values.</param>
		/// <param name="values">The values to write.</param>
		public void WriteDoubleArray( Path path, string containerName, double[] values )
		{
			TagList container = new TagList( containerName );
			for( int i = 0; i < values.Length; i++ )
			{
				container.Add( new PayloadDecimal( values[i] ) );
			}
			this.__WriteTag( path, false, container );
		}

		/// <summary>
		/// Writes a string value at the specified path.
		/// </summary>
		/// <param name="path">The path to write the Tag at (points to the container).</param>
		/// <param name="name">The name of the new Tag.</param>
		/// <param name="value">The value to write.</param>
		public void WriteString( Path path, string name, string value ) // writes a tag named 'name' to the class at path 'path'.
		{
			if( value == null )
			{
				throw new ArgumentNullException( "Can't write a null value." );
			}
			this.__WriteTag( path, false, new TagString( name, value ) );
		}

		/// <summary>
		/// Writes an array of string values at the specified path.
		/// </summary>
		/// <param name="path">The path to write the TagList at (points to the container).</param>
		/// <param name="containerName">The name of the TagList containing the values.</param>
		/// <param name="values">The values to write.</param>
		public void WriteStringArray( Path path, string containerName, string[] values )
		{
			if( values == null )
			{
				throw new ArgumentNullException( "'values' can't be null." );
			}
			TagList container = new TagList( containerName );
			for( int i = 0; i < values.Length; i++ )
			{
				if( values[i] == null )
				{
					throw new ArgumentNullException( "Can't write a null value." );
				}
				container.Add( new PayloadString( values[i] ) );
			}
			this.__WriteTag( path, false, container );
		}

		/// <summary>
		/// Writes a char value at the specified path.
		/// </summary>
		/// <param name="path">The path to write the Tag at (points to the container).</param>
		/// <param name="name">The name of the new Tag.</param>
		/// <param name="value">The value to write.</param>
		public void WriteChar( Path path, string name, char value ) // writes a tag named 'name' to the class at path 'path'.
		{
			this.__WriteTag( path, false, new TagString( name, value.ToString() ) );
		}

		/// <summary>
		/// Writes an array of char values at the specified path.
		/// </summary>
		/// <param name="path">The path to write the TagList at (points to the container).</param>
		/// <param name="containerName">The name of the TagList containing the values.</param>
		/// <param name="values">The values to write.</param>
		public void WriteCharArray( Path path, string containerName, char[] values )
		{
			TagList container = new TagList( containerName );
			for( int i = 0; i < values.Length; i++ )
			{
				container.Add( new PayloadString( values[i].ToString() ) );
			}
			this.__WriteTag( path, false, container );
		}


		/// <summary>
		/// Writes a class at the specified path.
		/// </summary>
		/// <param name="path">The path to write the Tag at (points to the container).</param>
		/// <param name="name">The name of the new TagClass.</param>
		public void WriteClass( Path path, string name )
		{
			this.__WriteTag( path, false, new TagClass( name ) );
		}

		/// <summary>
		/// Appends a Payload of type Class, to the list at the specified path.
		/// </summary>
		/// <param name="path">The path to the list.</param>
		public void AppendClass( Path path )
		{
			this.__AppendPayload( path, false, new PayloadClass() );
		}

		/// <summary>
		/// Writes a list at the specified path.
		/// </summary>
		/// <param name="path">The path to write the Tag at (points to the container).</param>
		/// <param name="name">The name of the new TagList.</param>
		public void WriteList( Path path, string name )
		{
			this.__WriteTag( path, false, new TagList( name ) );
		}

		/// <summary>
		/// Appends a Payload of type List, to the list at the specified path.
		/// </summary>
		/// <param name="path">The path to the list.</param>
		public void AppendList( Path path )
		{
			this.__AppendPayload( path, false, new PayloadList() );
		}

		/// <summary>
		/// Writes a serializable value at the specified path (calls IKFFSerializable.SerializeKFF on the value).
		/// </summary>
		/// <param name="path">The path to write the Tag at (points to the container).</param>
		/// <param name="name">The name of the new Tag.</param>
		/// <param name="serializableObj">The value to serialize.</param>
		public void Serialize<T>( Path path, string name, T serializableObj ) where T : IKFFSerializable, new()
		{
			// Be sure to set the scope to the 'serializedClass'!
			// save the scope locally, so we can put the file, after serialization, still being relative to it.
			Object beginScope = this.scopeRoot;
			TagClass serializedClass = new TagClass( name ); // create a new class and serialize into it.
			this.scopeRoot = serializedClass;
			serializableObj.SerializeKFF( this );
			// revert the scope to the locally-saved one.
			this.scopeRoot = beginScope;
			// write the file still relative to the scope right when the function was called.
			this.__WriteTag( path, false, serializedClass );
		}

		/// <summary>
		/// Writes an array of serializable values at the specified path (calls IKFFSerializable.SerializeKFF on each value).
		/// </summary>
		/// <param name="path">The path to write the TagList at (points to the container).</param>
		/// <param name="containerName">The name of the TagList containing the values.</param>
		/// <param name="serializableObjs">The values to serialize.</param>
		public void SerializeArray<T>( Path path, string containerName, T[] serializableObjs ) where T : IKFFSerializable, new()
		{
			// Be sure to set the scope to the 'serializedClass'!
			// save the scope locally, so we can put the file, after serialization, still being relative to it.
			Object beginScope = this.scopeRoot;
			TagList container = new TagList( containerName );
			for( int i = 0; i < serializableObjs.Length; i++ )
			{
				PayloadClass serializedClass = new PayloadClass();
				this.scopeRoot = serializedClass;
				serializableObjs[i].SerializeKFF( this );
				container.Add( serializedClass );
			}
			// write the file still relative to the scope right when the function was called.
			this.scopeRoot = beginScope;
			this.__WriteTag( path, false, container );
		}





		//#################################################################################################################################################################

		//#################################################################################################################################################################
		//
		//				READING
		//
		//#################################################################################################################################################################

		//#################################################################################################################################################################



		private bool __ReadBoolean( Object obj )
		{
			if( obj.isPayload )
			{
				return (PayloadBoolean)obj;
			}
			if( obj.isTag )
			{
				return ((TagBoolean)obj).payload;
			}
			throw new KFFReadWriteException( "Expected to find a Tag or a Payload, '" + obj.GetType().ToString() + "' was found instead." );
		}

		private long __ReadInteger( Object obj )
		{
			if( obj.isPayload )
			{
				return (PayloadInteger)obj;
			}
			if( obj.isTag )
			{
				return ((TagInteger)obj).payload;
			}
			throw new KFFReadWriteException( "Expected to find a Tag or a Payload, '" + obj.GetType().ToString() + "' was found instead." );
		}

		private double __ReadDecimal( Object obj )
		{
			if( obj.isPayload )
			{
				return (PayloadDecimal)obj;
			}
			if( obj.isTag )
			{
				return ((TagDecimal)obj).payload;
			}
			throw new KFFReadWriteException( "Expected to find a Tag or a Payload, '" + obj.GetType().ToString() + "' was found instead." );
		}

		private string __ReadString( Object obj )
		{
			if( obj.isPayload )
			{
				return (PayloadString)obj;
			}
			if( obj.isTag )
			{
				return ((TagString)obj).payload;
			}
			throw new KFFReadWriteException( "Expected to find a Tag or a Payload, '" + obj.GetType().ToString() + "' was found instead." );
		}


		public bool ReadBool( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.Boolean )
			{
				return this.__ReadBoolean( obj );
			}
			throw new KFFReadWriteException( "The object '" + obj.type.ToString() + "' can't be converted into 'bool'." );
		}

		public bool[] ReadBoolArray( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.List )
			{
				IList container = (IList)obj;
				if( container.listType == DataType.EmptyList )
				{
					return new bool[0];
				}
				if( container.listType == DataType.Boolean )
				{
					bool[] ret = new bool[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (PayloadBoolean)container.Get( i );
					}
					return ret;
				}
				throw new KFFReadWriteException( "The List found is not a list of type 'Boolean'." );
			}
			throw new KFFReadWriteException( "Expected to find a List, found '" + obj.GetType().ToString() + "' instead." );
		}

		public sbyte ReadSByte( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.Integer )
			{
				return (sbyte)this.__ReadInteger( obj );
			}
			if( obj.type == DataType.Decimal )
			{
				return (sbyte)this.__ReadDecimal( obj );
			}
			throw new KFFReadWriteException( "The object '" + obj.type.ToString() + "' can't be converted into 'sbyte'." );
		}

		public sbyte[] ReadSByteArray( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.List )
			{
				IList container = (IList)obj;
				if( container.listType == DataType.EmptyList )
				{
					return new sbyte[0];
				}
				if( container.listType == DataType.Integer )
				{
					sbyte[] ret = new sbyte[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (sbyte)(PayloadInteger)container.Get( i );
					}
					return ret;
				}
				if( container.listType == DataType.Decimal )
				{
					sbyte[] ret = new sbyte[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (sbyte)(PayloadDecimal)container.Get( i );
					}
					return ret;
				}
				throw new KFFReadWriteException( "The List found is not a list of type 'Integer' or 'Decimal'." );
			}
			throw new KFFReadWriteException( "Expected to find a List, found '" + obj.GetType().ToString() + "' instead." );
		}

		public byte ReadByte( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.Integer )
			{
				return (byte)this.__ReadInteger( obj );
			}
			if( obj.type == DataType.Decimal )
			{
				return (byte)this.__ReadDecimal( obj );
			}
			throw new KFFReadWriteException( "The object '" + obj.type.ToString() + "' can't be converted into 'byte'." );
		}

		public byte[] ReadByteArray( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.List )
			{
				IList container = (IList)obj;
				if( container.listType == DataType.EmptyList )
				{
					return new byte[0];
				}
				if( container.listType == DataType.Integer )
				{
					byte[] ret = new byte[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (byte)(PayloadInteger)container.Get( i );
					}
					return ret;
				}
				if( container.listType == DataType.Decimal )
				{
					byte[] ret = new byte[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (byte)(PayloadDecimal)container.Get( i );
					}
					return ret;
				}
				throw new KFFReadWriteException( "The List found is not a list of type 'Integer' or 'Decimal'." );
			}
			throw new KFFReadWriteException( "Expected to find a List, found '" + obj.GetType().ToString() + "' instead." );
		}

		public short ReadShort( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.Integer )
			{
				return (short)this.__ReadInteger( obj );
			}
			if( obj.type == DataType.Decimal )
			{
				return (short)this.__ReadDecimal( obj );
			}
			throw new KFFReadWriteException( "The object '" + obj.type.ToString() + "' can't be converted into 'short'." );
		}

		public short[] ReadShortArray( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.List )
			{
				IList container = (IList)obj;
				if( container.listType == DataType.EmptyList )
				{
					return new short[0];
				}
				if( container.listType == DataType.Integer )
				{
					short[] ret = new short[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (short)(PayloadInteger)container.Get( i );
					}
					return ret;
				}
				if( container.listType == DataType.Decimal )
				{
					short[] ret = new short[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (short)(PayloadDecimal)container.Get( i );
					}
					return ret;
				}
				throw new KFFReadWriteException( "The List found is not a list of type 'Integer' or 'Decimal'." );
			}
			throw new KFFReadWriteException( "Expected to find a List, found '" + obj.GetType().ToString() + "' instead." );
		}

		public ushort ReadUShort( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.Integer )
			{
				return (ushort)this.__ReadInteger( obj );
			}
			if( obj.type == DataType.Decimal )
			{
				return (ushort)this.__ReadDecimal( obj );
			}
			throw new KFFReadWriteException( "The object '" + obj.type.ToString() + "' can't be converted into 'ushort'." );
		}

		public ushort[] ReadUShortArray( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.List )
			{
				IList container = (IList)obj;
				if( container.listType == DataType.EmptyList )
				{
					return new ushort[0];
				}
				if( container.listType == DataType.Integer )
				{
					ushort[] ret = new ushort[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (ushort)(PayloadInteger)container.Get( i );
					}
					return ret;
				}
				if( container.listType == DataType.Decimal )
				{
					ushort[] ret = new ushort[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (ushort)(PayloadDecimal)container.Get( i );
					}
					return ret;
				}
				throw new KFFReadWriteException( "The List found is not a list of type 'Integer' or 'Decimal'." );
			}
			throw new KFFReadWriteException( "Expected to find a List, found '" + obj.GetType().ToString() + "' instead." );
		}

		public int ReadInt( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.Integer )
			{
				return (int)this.__ReadInteger( obj );
			}
			if( obj.type == DataType.Decimal )
			{
				return (int)this.__ReadDecimal( obj );
			}
			throw new KFFReadWriteException( "The object '" + obj.type.ToString() + "' can't be converted into 'int'." );
		}

		public int[] ReadIntArray( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.List )
			{
				IList container = (IList)obj;
				if( container.listType == DataType.EmptyList )
				{
					return new int[0];
				}
				if( container.listType == DataType.Integer )
				{
					int[] ret = new int[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (int)(PayloadInteger)container.Get( i );
					}
					return ret;
				}
				if( container.listType == DataType.Decimal )
				{
					int[] ret = new int[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (int)(PayloadDecimal)container.Get( i );
					}
					return ret;
				}
				throw new KFFReadWriteException( "The List found is not a list of type 'Integer' or 'Decimal'." );
			}
			throw new KFFReadWriteException( "Expected to find a List, found '" + obj.GetType().ToString() + "' instead." );
		}

		public uint ReadUInt( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.Integer )
			{
				return (uint)this.__ReadInteger( obj );
			}
			if( obj.type == DataType.Decimal )
			{
				return (uint)this.__ReadDecimal( obj );
			}
			throw new KFFReadWriteException( "The object '" + obj.type.ToString() + "' can't be converted into 'uint'." );
		}

		public uint[] ReadUIntArray( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.List )
			{
				IList container = (IList)obj;
				if( container.listType == DataType.EmptyList )
				{
					return new uint[0];
				}
				if( container.listType == DataType.Integer )
				{
					uint[] ret = new uint[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (uint)(PayloadInteger)container.Get( i );
					}
					return ret;
				}
				if( container.listType == DataType.Decimal )
				{
					uint[] ret = new uint[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (uint)(PayloadDecimal)container.Get( i );
					}
					return ret;
				}
				throw new KFFReadWriteException( "The List found is not a list of type 'Integer' or 'Decimal'." );
			}
			throw new KFFReadWriteException( "Expected to find a List, found '" + obj.GetType().ToString() + "' instead." );
		}

		public long ReadLong( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.Integer )
			{
				return (long)this.__ReadInteger( obj );
			}
			if( obj.type == DataType.Decimal )
			{
				return (long)this.__ReadDecimal( obj );
			}
			throw new KFFReadWriteException( "The object '" + obj.type.ToString() + "' can't be converted into 'long'." );
		}

		public long[] ReadLongArray( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.List )
			{
				IList container = (IList)obj;
				if( container.listType == DataType.EmptyList )
				{
					return new long[0];
				}
				if( container.listType == DataType.Integer )
				{
					long[] ret = new long[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (long)(PayloadInteger)container.Get( i );
					}
					return ret;
				}
				if( container.listType == DataType.Decimal )
				{
					long[] ret = new long[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (long)(PayloadDecimal)container.Get( i );
					}
					return ret;
				}
				throw new KFFReadWriteException( "The List found is not a list of type 'Integer' or 'Decimal'." );
			}
			throw new KFFReadWriteException( "Expected to find a List, found '" + obj.GetType().ToString() + "' instead." );
		}

		public ulong ReadULong( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.Integer )
			{
				return (ulong)this.__ReadInteger( obj );
			}
			if( obj.type == DataType.Decimal )
			{
				return (ulong)this.__ReadDecimal( obj );
			}
			throw new KFFReadWriteException( "The object '" + obj.type.ToString() + "' can't be converted into 'ulong'." );
		}

		public ulong[] ReadULongArray( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.List )
			{
				IList container = (IList)obj;
				if( container.listType == DataType.EmptyList )
				{
					return new ulong[0];
				}
				if( container.listType == DataType.Integer )
				{
					ulong[] ret = new ulong[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (ulong)(long)(PayloadInteger)container.Get( i );
					}
					return ret;
				}
				if( container.listType == DataType.Decimal )
				{
					ulong[] ret = new ulong[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (ulong)(PayloadDecimal)container.Get( i );
					}
					return ret;
				}
				throw new KFFReadWriteException( "The List found is not a list of type 'Integer' or 'Decimal'." );
			}
			throw new KFFReadWriteException( "Expected to find a List, found '" + obj.GetType().ToString() + "' instead." );
		}

		public float ReadFloat( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.Integer )
			{
				return (float)this.__ReadInteger( obj );
			}
			if( obj.type == DataType.Decimal )
			{
				return (float)this.__ReadDecimal( obj );
			}
			throw new KFFReadWriteException( "The object '" + obj.type.ToString() + "' can't be converted into 'float'." );
		}

		public float[] ReadFloatArray( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.List )
			{
				IList container = (IList)obj;
				if( container.listType == DataType.EmptyList )
				{
					return new float[0];
				}
				if( container.listType == DataType.Integer )
				{
					float[] ret = new float[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (float)(PayloadInteger)container.Get( i );
					}
					return ret;
				}
				if( container.listType == DataType.Decimal )
				{
					float[] ret = new float[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (float)(PayloadDecimal)container.Get( i );
					}
					return ret;
				}
				throw new KFFReadWriteException( "The List found is not a list of type 'Integer' or 'Decimal'." );
			}
			throw new KFFReadWriteException( "Expected to find a List, found '" + obj.GetType().ToString() + "' instead." );
		}

		public double ReadDouble( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.Integer )
			{
				return (double)this.__ReadInteger( obj );
			}
			if( obj.type == DataType.Decimal )
			{
				return (double)this.__ReadDecimal( obj );
			}
			throw new KFFReadWriteException( "The object '" + obj.type.ToString() + "' can't be converted into 'double'." );
		}

		public double[] ReadDoubleArray( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.List )
			{
				IList container = (IList)obj;
				if( container.listType == DataType.EmptyList )
				{
					return new double[0];
				}
				if( container.listType == DataType.Integer )
				{
					double[] ret = new double[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (double)(PayloadInteger)container.Get( i );
					}
					return ret;
				}
				if( container.listType == DataType.Decimal )
				{
					double[] ret = new double[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (double)(PayloadDecimal)container.Get( i );
					}
					return ret;
				}
				throw new KFFReadWriteException( "The List found is not a list of type 'Integer' or 'Decimal'." );
			}
			throw new KFFReadWriteException( "Expected to find a List, found '" + obj.GetType().ToString() + "' instead." );
		}

		public char ReadChar( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.String )
			{
				if( obj.isPayload )
				{
					string s = (string)((PayloadString)obj);
					if( string.IsNullOrEmpty( s ) || s.Length != 1 )
					{
						throw new KFFReadWriteException( "Can't convert string '" + s + "' into a char." );
					}
					return s[0];
				}
				if( obj.isTag )
				{
					string s = (string)((TagString)obj).payload;
					if( string.IsNullOrEmpty( s ) || s.Length != 1 )
					{
						throw new KFFReadWriteException( "Can't convert string '" + s + "' into a char." );
					}
					return s[0];
				}
				throw new KFFReadWriteException( "Expected to find a Tag or a Payload, '" + obj.GetType().ToString() + "' was found instead." );
			}
			throw new KFFReadWriteException( "The object '" + obj.type.ToString() + "' can't be converted into 'string'." );
		}

		public char[] ReadCharArray( Path path ) // converts array of single-character strings into array of char values.
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.List )
			{
				IList container = (IList)obj;
				if( container.listType == DataType.EmptyList )
				{
					return new char[0];
				}
				if( container.listType == DataType.String )
				{
					char[] ret = new char[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						string s = (string)((PayloadString)obj);
						if( string.IsNullOrEmpty( s ) || s.Length != 1 )
						{
							throw new KFFReadWriteException( "Can't convert string '" + s + "' into a char." );
						}
						ret[i] = s[0];
					}
					return ret;
				}
				throw new KFFReadWriteException( "The List found is not a list of type 'Integer' or 'Decimal'." );
			}
			throw new KFFReadWriteException( "Expected to find a List, found '" + obj.GetType().ToString() + "' instead." );
		}

		public string ReadString( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.String )
			{
				return this.__ReadString( obj );
			}
			throw new KFFReadWriteException( "The object '" + obj.type.ToString() + "' can't be converted into 'string'." );
		}

		public string[] ReadStringArray( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.List )
			{
				IList container = (IList)obj;
				if( container.listType == DataType.EmptyList )
				{
					return new string[0];
				}
				if( container.listType == DataType.String )
				{
					string[] ret = new string[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (string)(PayloadString)container.Get( i );
					}
					return ret;
				}
				throw new KFFReadWriteException( "The List found is not a list of type 'String'." );
			}
			throw new KFFReadWriteException( "Expected to find a List, found '" + obj.GetType().ToString() + "' instead." );
		}

		public decimal ReadDecimal( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.Integer )
			{
				return (decimal)this.__ReadInteger( obj );
			}
			if( obj.type == DataType.Decimal )
			{
				return (decimal)this.__ReadDecimal( obj );
			}
			throw new KFFReadWriteException( "The object '" + obj.type.ToString() + "' can't be converted into 'decimal'." );
		}

		public decimal[] ReadDecimalArray( Path path )
		{
			Object obj = this.MoveScope( path, false );
			if( obj.type == DataType.List )
			{
				IList container = (IList)obj;
				if( container.listType == DataType.EmptyList )
				{
					return new decimal[0];
				}
				if( container.listType == DataType.Integer )
				{
					decimal[] ret = new decimal[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (decimal)(PayloadInteger)container.Get( i );
					}
					return ret;
				}
				if( container.listType == DataType.Decimal )
				{
					decimal[] ret = new decimal[container.count];
					for( int i = 0; i < ret.Length; i++ )
					{
						ret[i] = (decimal)(double)(PayloadDecimal)container.Get( i );
					}
					return ret;
				}
				throw new KFFReadWriteException( "The list at the end of the path is not a list of numbers." );
			}
			throw new KFFReadWriteException( "Expected to find a List, found '" + obj.GetType().ToString() + "' instead." );
		}

		public void Deserialize<T>( Path path, T serializableObj ) where T : IKFFSerializable, new()
		{
			// save the scope locally and modify the real one.
			Object beginScope = this.scopeRoot;
			this.MoveScope( path, true );
			serializableObj = new T();
			serializableObj.DeserializeKFF( this );
			// revert the scope to the locally-saved one.
			this.scopeRoot = beginScope;
		}

		public void DeserializeArray<T>( Path path, T[] serializableObj ) where T : IKFFSerializable, new()
		{
			// save the scope locally and modify the real one.
			Object beginScope = this.scopeRoot;
			Object list = this.MoveScope( path, true ); // save the scope of the list.
			if( list.type != DataType.List )
			{
				throw new KFFReadWriteException( "The object '" + list.GetType().ToString() + "' is not a list." );
			}
			serializableObj = new T[((IList)list).count];
			for( int i = 0; i < serializableObj.Length; i++ )
			{
				serializableObj[i] = new T();
				this.MoveScope( i.ToString( Syntax.numberFormat ), true ); // move scope to the i'th element of the list.
				serializableObj[i].DeserializeKFF( this );
				this.scopeRoot = list; // move the scope back to the list.
			}
			// revert the scope to the locally-saved one.
			this.scopeRoot = beginScope;
		}
	}
}