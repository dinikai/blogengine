using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogEngine
{
    internal class Note
    {
        public int Id { get; set; }
        public string Title { get; set; } = "New note";
        public string Text { get; set; } = "What's do you think?";
        public DateTime LastChanged { get; set; }
        public RenderType RenderType { get; set; }
        public NoteStatus NoteStatus { get; set; }
        public bool AllowComments { get; set; }

        public Note()
        {
            LastChanged = DateTime.Now;
            RenderType = RenderType.Markdown;
            NoteStatus = NoteStatus.Draft;
            AllowComments = true;
        }

        public string FormatTitle() => Title.Trim('(', ')');
        public string FormatHeader() => Title.StartsWith('(') && Title.EndsWith(')') ? "" : Title;
    }
}
