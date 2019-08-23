namespace KFF.DataStructures
{
	/// <summary>
	/// An abstract class representing any object inside of the KFF file.
	/// </summary>
	public abstract class Object
	{
		/// <summary>
		/// Contains the parent-object of this object (Read Only).
		/// </summary>
		internal Object parent { get; set; }

		/// <summary>
		/// Returns true if this object has a parent-object, false otherwise (Read Only).
		/// </summary>
		internal bool hasParent
		{
			get
			{
				return this.parent != null;
			}
		}

		/// <summary>
		/// Returns the data type of this Payload (Read Only).
		/// </summary>
		public abstract DataType type { get; }

		/// <summary>
		/// Returns true if this object is a Tag, false otherwise (Read Only).
		/// </summary>
		public abstract bool isTag { get; }

		/// <summary>
		/// Returns true if this object is a Payload, false otherwise (Read Only).
		/// </summary>
		public abstract bool isPayload { get; }



		/// <summary>
		/// Creates a new object with the specified object as parent.
		/// </summary>
		protected Object()
		{
			this.parent = null;
		}
	}
}