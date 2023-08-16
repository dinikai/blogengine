using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogEngine.Database
{
    internal class Comment
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int NoteId { get; set; }
        public string Author { get; set; } = "";
        public string Text { get; set; } = "";
        public DateTime DateTime { get; set; }
        public CommentStatus Status { get; set; }

        public Comment()
        {
            DateTime = DateTime.Now;
            Status = CommentStatus.Waiting;
        }
    }
}
