using BlogEngine.Database;
using HttpEngine.Core;
using System;
using System.Globalization;

namespace BlogEngine.Models
{
    internal class NoteModel : Model
    {
        public NoteModel()
        {
            Routes = new()
            {
                "/note/{id}"
            };
        }

        public override ModelResult OnRequest(ModelRequest request)
        {
            byte[] buffer = File($"themes/{BlogConfig.Theme}/note.html", request);

            if (!request.Arguments.Arguments.ContainsKey("id"))
                return Skip();
            int id;
            try
            {
                id = Convert.ToInt32(request.Arguments.Arguments["id"]);
            }
            catch
            {
                return Skip();
            }

            using BlogContext db = new BlogContext();
            Note? note = db.Notes.FirstOrDefault(x => x.Id == id);

            if (note == null)
                return Skip();
            if (note.NoteStatus != NoteStatus.Public)
                return Skip();

            string commentMessage = "";
            bool allowComments = note.AllowComments && BlogConfig.AllowComments;
            if (request.Handler == "new_comment" && allowComments)
            {
                try
                {
                    CommentStatus status = BlogConfig.ModerateComments ? CommentStatus.Waiting : CommentStatus.Allowed;
                    int parentId = -1;
                    if (request.Arguments.Arguments.ContainsKey("reply"))
                    {
                        try
                        {

                            if (db.Comments.FirstOrDefault(x => x.Id == Convert.ToInt32(request.Arguments.Arguments["reply"])) != null)
                                parentId = Convert.ToInt32(request.Arguments.Arguments["reply"]);
                            status = CommentStatus.Allowed;
                        }
                        catch { }
                    } else
                    {
                        if (BlogConfig.ModerateComments)
                            commentMessage = buffer.GetSection("addCommentMessage");
                    }

                    db.Comments.Add(new Comment()
                    {
                        Author = request.Arguments.Arguments["author"],
                        Text = request.Arguments.Arguments["text"],
                        NoteId = note.Id,
                        ParentId = parentId,
                        Status = status
                    });
                    db.SaveChanges();
                }
                catch { }
            }

            string text = "invalid content type";
            switch (note.RenderType)
            {
                case RenderType.Markdown:
                    text = Markdown.ToHtml(note.Text);
                    break;
                case RenderType.Html:
                    text = note.Text;
                    break;
            }

            string jsComments = "";
            string commentsString = buffer.GetSection("commentsNotAllowed");
            if (allowComments)
            {
                List<Comment> comments = db.Comments.Where(x => x.NoteId == note.Id && x.Status == CommentStatus.Allowed).ToList();
                List<RankedComment> rankedComments = new();

                foreach (Comment comment in comments)
                {
                    if (comment.ParentId == -1)
                        rankedComments.Add(EnumerateComment(comment, db));
                    jsComments += buffer.GetSection("jsComment", new()
                    {
                        ["author"] = comment.Author,
                        ["text"] = comment.Text,
                        ["id"] = comment.Id
                    });
                }
                commentsString = RenderComments(rankedComments.ToArray(), buffer);
            }

            buffer = buffer.ParseView(new()
            {
                ["commentForm"] = allowComments ? buffer.GetSection("commentForm") : ""
            }).ParseView(new()
            {
                ["id"] = note.Id,
                ["title"] = note.Title,
                ["name"] = note.Title,
                ["text"] = text,
                ["lastChanged"] = note.LastChanged.ToString("HH:mm dd.MM.yyyy"),
                ["comments"] = commentsString,
                ["jsComments"] = jsComments,
                ["commentMessage"] = commentMessage
            });

            return new ModelResult(buffer);
        }

        public static RankedComment EnumerateComment(Comment parent, BlogContext db, int rank = 0)
        {
            RankedComment comment = new RankedComment(parent)
            {
                Rank = rank
            };
            List<Comment> children = db.Comments.Where(x => x.ParentId == parent.Id).ToList();
            List<RankedComment> allChildren = new();

            foreach (Comment child in children)
            {
                allChildren.Add(EnumerateComment(child, db, rank + 1));
            }
            comment.Children = allChildren.ToArray();

            return comment;
        }

        public static string RenderComments(RankedComment[] comments, byte[] buffer)
        {
            string commentsString = "";
            foreach (RankedComment comment in comments)
            {
                commentsString += buffer.GetSection("comment", new()
                {
                    ["rank"] = comment.Rank,
                    ["author"] = comment.Author,
                    ["text"] = comment.Text,
                    ["id"] = comment.Id,
                    ["date"] = comment.DateTime.ToString("dd.MM.yyyy HH:mm")
                });
                commentsString += RenderComments(comment.Children.ToArray(), buffer);
            }

            return commentsString;
        }
    }
}
