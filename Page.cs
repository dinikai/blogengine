using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogEngine
{
    internal class Page
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Content { get; set; }
        public RenderType RenderType { get; set; }

        public string FormatTitle() => Name.Trim('(', ')');
        public string FormatHeader() => Name.StartsWith('(') && Name.EndsWith(')') ? "" : Name;
    }
}
