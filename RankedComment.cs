using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlogEngine
{
    internal class RankedComment : Comment
    {
        public int Rank { get; set; }
        public RankedComment[] Children { get; set; }

        public RankedComment(Comment comment)
        {
            foreach (PropertyInfo property in typeof(Comment).GetProperties())
                typeof(RankedComment).GetProperty(property.Name)!.SetValue(this, property.GetValue(comment));

            Rank = 0;
            Children = Array.Empty<RankedComment>();
        }
    }
}
