using KFF.DataStructures;
using System;
using Object = KFF.DataStructures.Object;

namespace KFF
{
	/// <summary>
	/// Represents a parsed KFF file.
	/// </summary>
	public class KFFFile : Object, IClass
	{
		/// <summary>
		/// The name of the file. Displayed when exceptions are thrown.
		/// </summary>
		public string fileName { get; set; }
		
		/// <summary>
		/// The tags contained directly in the file (nest = 0).
		/// </summary>
		public PayloadClass tags { get; set; }
		
		/// <summary>
		/// The amount of tags directly in the file (nest = 0).
		/// </summary>
		public int count
		{
			get
			{
				return this.tags.count;
			}
		}

		/// <summary>
		/// Returns the type of this Payload (Read Only).
		/// </summary>
		public override DataType type
		{
			get
			{
				return DataType.Class;
			}
		}

		/// <summary>
		/// Returns true if this Object is a Tag, false otherwise (Read Only).
		/// </summary>
		public override bool isTag
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Returns true if this Object is a Payload, false otherwise (Read Only).
		/// </summary>
		public override bool isPayload
		{
			get
			{
				return false;
			}
		}



		/// <summary>
		/// Creates a new KFF file with the specified tags inside.
		/// </summary>
		public KFFFile( string fileName )
		{
			this.fileName = fileName;
			this.tags = new PayloadClass();
			this.tags.EmbedIn( this );
		}

		/// <summary>
		/// Checks if the class contains a tag with the specified name.
		/// </summary>
		/// <param name="name">The name to check.</param>
		public bool Has( string name )
		{
			return this.tags.Has( name );
		}

		/// <summary>
		/// Tries to get the tag with the specified name, as a generic Tag object. Returns true if the tag was found.
		/// </summary>
		/// <param name="name">The name of the tag to get.</param>
		/// <param name="t">The variable to store the tag into. Is going to be null if the operation fails.</param>
		public bool TryGet( string name, out Tag t )
		{
			return this.tags.TryGet( name, out t );
		}

		/// <summary>
		/// Gets the tag with the specified name, as a generic Tag object.
		/// </summary>
		/// <param name="name">The name of the tag to get.</param>
		/// <exception cref="KFFObjectPresenceException">Thrown when the tag with the specified name isn't present in the class.</exception>
		public Tag Get( string name )
		{
			return this.tags.Get( name );
		}

		/// <summary>
		/// Gets all the tags in the class, as a generic Tag object array.
		/// </summary>
		public Tag[] GetAll()
		{
			return this.tags.GetAll();
		}
		
		/// <summary>
		/// Sets the tags with the specified names. Replaces the tag if already present, adds a new tag otherwise.
		/// </summary>
		/// <param name="t">The new tags to add to the class.</param>
		public void Set( params Tag[] t )
		{
			this.tags.Set( t );
		}

		/// <summary>
		/// Removes a tag with the specified name from the class.
		/// </summary>
		/// <param name="name">The name of the tag to remove.</param>
		public void Remove( string name )
		{
			this.tags.Remove( name );
		}

		/// <summary>
		/// Removes all tags from the class.
		/// </summary>
		public void Clear()
		{
			this.tags.Clear();
		}


		/// <summary>
		/// Returns a tag or payload at the specified path.
		/// </summary>
		/// <param name="path">The path to get the tag/payload at.</param>
		/// <exception cref="KFFException">Thrown when the path can't be resolved.</exception>
		[Obsolete]
		public object PathFind( Path path ) // returns Tag or Payload.
		{
			if( path.isEmpty )
			{
				return this;
			}

			IClass currentClass = this;
			IList currentList = null;
			int lastSegmentIndex = path.nestLevel - 1;

			for( int i = 0; i < lastSegmentIndex; i++ )
			{
				if( path[i].destination == ObjectType.Tag )
				{
					if( currentClass == null )
					{
						throw new KFFException( "Invalid path, can't find Tag with the name '" + path[i].name + "' (" + this.fileName + ")." );
					}
					if( currentClass.TryGet( path[i].name, out Tag t ) )
					{
						currentClass = t as IClass;
						currentList = t as IList;
					}
					else
					{
						throw new KFFException( "Can't find Tag with the name '" + path[i].name + "' inside of the class (" + this.fileName + ")." );
					}
				}
				else // destination = payload
				{
					if( currentList == null )
					{
						throw new KFFException( "Invalid path, can't find Payload with the index '" + i + "' (" + this.fileName + ")." );
					}
					if( currentList.TryGet( path[i].index, out Payload t ) )
					{
						currentClass = t as IClass;
						currentList = t as IList;
					}
					else
					{
						throw new KFFException( "Can't find Payload with the index '" + i + "' inside of the list (" + this.fileName + ")." );
					}
				}
			}
			if( currentClass != null )
			{
				if( currentClass.TryGet( path[lastSegmentIndex].name, out Tag t ) )
				{
					return t;
				}
				throw new KFFException( "Can't find Tag with the name '" + path[lastSegmentIndex].name + "' inside of the class (" + this.fileName + ")." );
			}
			if( currentList != null )
			{
				if( currentList.TryGet( path[lastSegmentIndex].index, out Payload p ) ) // don't fetch if checking last segment.
				{
					return p;
				}
				throw new KFFException( "Can't find Payload with the index '" + (lastSegmentIndex) + "' inside of the list (" + this.fileName + ")." );
			}
			throw new KFFException( "Error, the specified path is invalid or the Tag/Payload in't present (" + this.fileName + ")." );
		}
	}
}