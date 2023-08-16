using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogEngine
{
    internal static class Extensions
    {
        public static string ToLiteral(this string str) => str.Replace("\"", "\\\"").Replace("'", "\\'")
            .Replace("\n", "\\n").Replace("\r", "\\r");
    }
}
