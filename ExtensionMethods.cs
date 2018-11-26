using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnroladorStandAlone
{
    public static class ExtensionMethods
    {
        public static string GetDisplayName(this Enum enumType)
        {
            var displayNameAttribute = enumType.GetType().GetField(enumType.ToString()).GetCustomAttributes(typeof(EnumDisplayNameAttribute), false).FirstOrDefault() as EnumDisplayNameAttribute;

            return displayNameAttribute != null ? displayNameAttribute.DisplayName : Enum.GetName(enumType.GetType(), enumType);
        }
    }
}
