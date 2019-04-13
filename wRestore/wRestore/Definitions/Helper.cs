using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wRestore.Definitions
{
    public static class Helper
    {
        public static object[] StringToArray(string input, string separator, Type type)
        {
            string[] stringList = input.Split(separator.ToCharArray(),
                                              StringSplitOptions.RemoveEmptyEntries);
            object[] list = new object[stringList.Length];

            for (int i = 0; i < stringList.Length; i++)
            {
                list[i] = Convert.ChangeType(stringList[i], type);
            }

            return list;
        }
    }
}
