using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FGA.Automate.Helpers
{
    internal sealed class EnumConverter : ConverterBase
    {
        readonly Type mEnumType;

        public EnumConverter(Type sourceEnum)
        {
            if (sourceEnum.IsEnum == false)
                throw new Exception("The sourceType must be an Enum and is of type " + sourceEnum.Name);

            mEnumType = sourceEnum;
        }

        public override object StringToField(string from)
        {
            try
            {
                return Enum.Parse(mEnumType, from.Trim(), true);
            }
            catch (ArgumentException)
            {
                throw new ConvertException(from, mEnumType, "The value don't is on the Enum.");
            }
        }

    }
}
