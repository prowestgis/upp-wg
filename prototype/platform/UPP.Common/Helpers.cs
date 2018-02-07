using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPP.Common
{
    public static class Helpers
    {
        public static IEnumerable<string> FromCSV(this string str)
        {
            return str.Split(',').Select(x => x.Trim());
        }
    }
}
