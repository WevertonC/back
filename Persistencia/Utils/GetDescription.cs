using System;
using System.ComponentModel;
using System.Reflection;

namespace Persistencia.Utils
{
    public class GetDescription
    {
        public static string GetDescriptionEnum(Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            object[] attribs = field.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attribs.Length > 0)
            {
                return ((DescriptionAttribute)attribs[0]).Description.ToString();
            }
            return string.Empty;
        }

    }
}
