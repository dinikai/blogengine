using HttpEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogEngine.Models.Admin
{
    internal class AdminConsoleModel : AdminModel
    {
        public AdminConsoleModel()
        {
            Routes = new()
            {
                "/admin",
                "/admin/console",
            };
        }

        public override void OnUse()
        {
            Layout = new AdminLayout(PublicDirectory);
        }

        public override ModelResult OnRequest(ModelRequest request)
        {
            if (!CheckLogin(request))
                return Skip();

            byte[] buffer = File("admin/console.html", request);

            using BlogContext db = new();
            List<Comment> lastComments = db.Comments.Where(x => x.ParentId == -1 && x.Status == CommentStatus.Waiting || x.Status == CommentStatus.Allowed).ToList();
            lastComments.Reverse();
            lastComments = lastComments.Take(5).ToList();

            string lastCommentsString = lastComments.Any() ? "" : buffer.GetSection("noLastComments");
            foreach (Comment comment in lastComments)
            {
                Note note = db.Notes.First(x => x.Id == comment.NoteId);
                lastCommentsString += buffer.GetSection("lastComment", new()
                {
                    ["author"] = comment.Author,
                    ["noteId"] = note.Id,
                    ["noteName"] = note.Title,
                    ["text"] = comment.Text,
                    ["date"] = comment.DateTime.ToString("HH:mm dd.MM.yyyy"),
                    ["id"] = comment.Id,
                    ["moderate"] = comment.Status == CommentStatus.Waiting ? buffer.GetSection("lastCommentModerate", new()
                    {
                        ["id"] = comment.Id
                    }) : ""
                });
            }

            buffer = buffer.ParseView(new()
            {
                ["title"] = "Console",
                ["notesCount"] = db.Notes.Count(),
                ["pagesCount"] = db.Pages.Count(),
                ["commentsCount"] = db.Comments.Where(x => x.ParentId == -1).Count(),
                ["lastComments"] = lastCommentsString
            });

            return new ModelResult(buffer);
        }
    }
}
