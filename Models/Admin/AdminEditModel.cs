using BlogEngine.Database;
using HttpEngine.Core;
using Markdig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogEngine.Models.Admin
{
    internal class AdminEditModel : AdminModel
    {
        public AdminEditModel()
        {
            Routes = new()
            {
                "/admin/notes/edit",
                "/admin/pages/edit"
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

            byte[] buffer;

            BlogContext db = new();
            int id;
            try
            {
                id = Convert.ToInt32(request.Arguments.Arguments["id"]);
            }
            catch
            {
                return Skip();
            }

            string header = "", messages = "";
            switch (request.Route)
            {
                case "/admin/pages/edit":
                    buffer = File("admin/pages/edit.html", request);

                    int pageNumber = 1;
                    Page? page = db.Pages.FirstOrDefault(x => x.Id == id);
                    if (page == null)
                        return Skip();
                    foreach (Page pageEach in db.Pages)
                        if (pageEach != page) pageNumber++;
                        else
                            break;

                    header = buffer.GetSection("pageHeader", new()
                    {
                        ["number"] = pageNumber,
                        ["id"] = page.Id,
                        ["page"] = page.Url,
                    });

                    messages = "";
                    if (request.Handler == "save")
                    {
                        try
                        {
                            page.Name = request.Arguments.Arguments["title"];
                            page.Content = request.Arguments.Arguments["text"];
                            page.Url = request.Arguments.Arguments["url"];

                            RenderType renderType = RenderType.Markdown;
                            switch (request.Arguments.Arguments["content_type"])
                            {
                                case "md":
                                    renderType = RenderType.Markdown;
                                    break;
                                case "html":
                                    renderType = RenderType.Html;
                                    break;
                            }
                            page.RenderType = renderType;
                            db.SaveChanges();

                            messages = buffer.GetSection("saveMessage");
                        }
                        catch
                        {
                            messages = buffer.GetSection("errorMessage");
                        }
                    }

                    buffer = buffer.ParseView(new()
                    {
                        ["title"] = $"Notes → Editing page {pageNumber}",
                        ["messages"] = messages,
                        ["id"] = page.Id,
                        ["pageUrl"] = page.Url,
                        ["pageNumber"] = pageNumber,
                        ["pageTitle"] = page.Name,
                        ["pageText"] = page.Content,
                        ["mdSelected"] = page.RenderType == RenderType.Markdown ? "selected" : "",
                        ["htmlSelected"] = page.RenderType == RenderType.Html ? "selected" : "",
                    });
                    break;
                case "/admin/notes/edit":
                    buffer = File("admin/notes/edit.html", request);

                    int noteNumber = 1;
                    Note? note = db.Notes.FirstOrDefault(x => x.Id == id);
                    if (note == null)
                        return Skip();
                    foreach (Note noteEach in db.Notes)
                        if (noteEach != note) noteNumber++;
                        else
                            break;

                    header = buffer.GetSection("noteHeader", new()
                    {
                        ["number"] = noteNumber,
                        ["id"] = note.Id,
                    });

                    messages = "";
                    if (request.Handler == "save")
                    {
                        try
                        {
                            note.Title = request.Arguments.Arguments["title"];
                            note.Text = request.Arguments.Arguments["text"];
                            note.LastChanged = DateTime.UtcNow;

                            RenderType renderType = RenderType.Markdown;
                            switch (request.Arguments.Arguments["content_type"])
                            {
                                case "md":
                                    renderType = RenderType.Markdown;
                                    break;
                                case "html":
                                    renderType = RenderType.Html;
                                    break;
                            }
                            note.RenderType = renderType;

                            NoteStatus noteStatus = NoteStatus.Private;
                            switch (request.Arguments.Arguments["status"])
                            {
                                case "draft":
                                    noteStatus = NoteStatus.Draft;
                                    break;
                                case "private":
                                    noteStatus = NoteStatus.Private;
                                    break;
                                case "public":
                                    noteStatus = NoteStatus.Public;
                                    break;
                            }
                            note.NoteStatus = noteStatus;

                            note.AllowComments = request.Arguments.Arguments.ContainsKey("allow_comments");
                            db.SaveChanges();

                            messages = buffer.GetSection("saveMessage");
                        }
                        catch
                        {
                            messages = buffer.GetSection("errorMessage");
                        }
                    }

                    buffer = buffer.ParseView(new()
                    {
                        ["title"] = $"Notes → Editing note {noteNumber}",
                        ["messages"] = messages,
                        ["id"] = note.Id,
                        ["noteNumber"] = noteNumber,
                        ["noteTitle"] = note.Title,
                        ["noteText"] = note.Text,
                        ["draftSelected"] = note.NoteStatus == NoteStatus.Draft ? "selected" : "",
                        ["privateSelected"] = note.NoteStatus == NoteStatus.Private ? "selected" : "",
                        ["publicSelected"] = note.NoteStatus == NoteStatus.Public ? "selected" : "",
                        ["mdSelected"] = note.RenderType == RenderType.Markdown ? "selected" : "",
                        ["htmlSelected"] = note.RenderType == RenderType.Html ? "selected" : "",
                        ["allowComments"] = note.AllowComments ? "checked" : ""
                    });
                    break;
                default:
                    return Skip();
            }
                
            
            return new ModelResult(buffer);
        }
    }
}
