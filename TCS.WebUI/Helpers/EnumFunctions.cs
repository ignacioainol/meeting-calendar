using System;
using System.Collections.Generic;

namespace TCS.WebUI.Helpers
{
    public static class EnumFunctions
    {
        public static List<EnumParameters> EnumToObjectList<T>()
        {
            List<EnumParameters> enums = new();
            foreach (object item in Enum.GetValues(typeof(T)))
            {
                EnumParameters enumElem = new();
                enumElem.Description = item.ToString();
                enumElem.Value = (int)item;
                enums.Add(enumElem);
            }
            return enums;
        }
    }
}
