#region "  � Copyright 2005-07 to Marcos Meli - http://www.devoo.net" 

// Errors, suggestions, contributions, send a mail to: marcos@filehelpers.com.

#endregion

using System;
using System.ComponentModel;

namespace FileHelpers
{
	/// <summary>Base class of <see cref="FieldFixedLengthAttribute"/> and <see cref="FieldDelimiterAttribute"/></summary>
	/// <remarks>See the <a href="attributes.html">Complete Attributes List</a> for more information and examples of each one.</remarks>
	/// <seealso href="attributes.html">Attributes list</seealso>
	/// <seealso href="quick_start.html">Quick start guide</seealso>
	/// <seealso href="examples.html">Examples of use</seealso>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class FieldAttribute : Attribute
	{
		/// <summary>Abstract class, see the derived classes.</summary>
		protected FieldAttribute()
		{
		}
	}
}