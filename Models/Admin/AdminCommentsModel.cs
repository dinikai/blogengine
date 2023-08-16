using HttpEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogEngine.Models.Admin
{
    internal class AdminCommentsModel : AdminModel
    {
        public AdminCommentsModel()
        {
            Routes = new()
            {
                "/admin/comments"
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

            byte[] buffer = File("admin/comments.html", request);

            using BlogContext db = new();
            List<Comment> rootComments = db.Comments.Where(x => x.ParentId == -1).ToList();
            rootComments.Reverse();

            string commentsString = rootComments.Any() ? "" : buffer.GetSection("noComments");
            int i = 0;
            foreach (Comment comment in rootComments)
            {
                Note note = db.Notes.First(x => x.Id == comment.NoteId);

                string moderate = "";
                switch (comment.Status)
                {
                    case CommentStatus.Blocked:
                        moderate = buffer.GetSection("commentModerateBlocked", new() { ["id"] = comment.Id });
                        break;
                    case CommentStatus.Waiting:
                        moderate = buffer.GetSection("commentModerate", new() { ["id"] = comment.Id });
                        break;
                    case CommentStatus.Allowed:
                        moderate = buffer.GetSection("commentModerateAllowed", new() { ["id"] = comment.Id });
                        break;
                }

                commentsString += buffer.GetSection("comment", new()
                {
                    ["noteName"] = note.Title,
                    ["id"] = comment.Id,
                    ["text"] = comment.Text,
                    ["author"] = comment.Author,
                    ["id"] = comment.Id,
                    ["moderate"] = moderate,
                    ["class"] = comment.Status == CommentStatus.Blocked ? "admin-comment-blocked" : ""
                });
                i++;
            }

            buffer = buffer.ParseView(new()
            {
                ["title"] = "Comments",
                ["rootCount"] = rootComments.Count,
                ["comments"] = commentsString
            });

            return new ModelResult(buffer);
        }
    }
}
