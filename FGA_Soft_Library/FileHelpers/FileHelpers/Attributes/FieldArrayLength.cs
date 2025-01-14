#region "  � Copyright 2005-07 to Marcos Meli - http://www.devoo.net" 

// Errors, suggestions, contributions, send a mail to: marcos@filehelpers.com.

#endregion

using System;

namespace FileHelpers
{
	/// <summary>
	/// Allow set the length or bounds that the target array field must have.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class FieldArrayLengthAttribute : Attribute
	{

		#region "  Constructors  "

		/// <summary>
		/// Allow set the bounds that the target array field must have.
		/// </summary>
		/// <param name="minLength">The lower bound</param>
		/// <param name="maxLength">The upper bound</param>
		public FieldArrayLengthAttribute(int minLength, int maxLength)
		{
			mMinLength = minLength;
			mMaxLength = maxLength;
		}

		/// <summary>
		/// Allow set the exact length that the target array field must have.
		/// </summary>
		/// <param name="length">The exact length of the array field.</param>
		public FieldArrayLengthAttribute(int length) : this(length, length)
		{
		}


		#endregion

		internal int mMinLength;
		internal int mMaxLength;
	}
}