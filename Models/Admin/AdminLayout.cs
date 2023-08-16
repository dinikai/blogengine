using BlogEngine.Database;
using HttpEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogEngine.Models.Admin
{
    internal class AdminLayout : Layout
    {
        public AdminLayout(string publicDirectory) : base(publicDirectory)
        {
        }

        public override byte[] OnRequest(ModelRequest request)
        {
            byte[] buffer = File("admin/_Layout.html");

            BlogContext db = new();

            buffer = buffer.ParseView(new()
            {
                ["notesCount"] = db.Notes.Count(),
                ["pagesCount"] = db.Pages.Count(),
                ["commentsCount"] = db.Comments.Where(x => x.ParentId == -1).Count()
            });

            return buffer;
        }
    }
}
