using BlogEngine.Database;
using HttpEngine.Core;
using System.Globalization;
using System.Text;

namespace BlogEngine.Models.Admin
{
    internal class AdminNotesModel : AdminModel
    {
        int maxNoteLength = 60;

        public AdminNotesModel()
        {
            Routes = new()
            {
                "/admin/notes",
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

            byte[] buffer = File("admin/notes/index.html", request);

            BlogContext db = new();

            string notesList = "";
            {
                int i = 0;
                foreach (Note note in db.Notes)
                {
                    string markup = "Markdown";
                    switch (note.RenderType)
                    {
                        case RenderType.Markdown:
                            markup = "Markdown";
                            break;
                        case RenderType.Html:
                            markup = "HTML";
                            break;
                        default:
                            break;
                    }

                    string status = "Private";
                    switch (note.NoteStatus)
                    {
                        case NoteStatus.Draft:
                            status = "<span class=\"icon-clock\"></span> Draft";
                            break;
                        case NoteStatus.Private:
                            status = "<span class=\"icon-key\"></span> Private";
                            break;
                        case NoteStatus.Public:
                            status = "<span class=\"icon-earth\"></span> Public";
                            break;
                    }

                    notesList += buffer.GetSection("note", new()
                    {
                        ["number"] = ++i,
                        ["id"] = note.Id,
                        ["title"] = note.Title,
                        ["markup"] = markup,
                        ["lastChanged"] = note.LastChanged.ToString("HH:mm dd.MM.yyyy"),
                        ["status"] = status,
                    });
                }
            }

            buffer = buffer.ParseView(new()
            {
                ["title"] = "Notes",
                ["notesCount"] = db.Notes.Count(),
                ["notesList"] = notesList,
            });

            return new ModelResult(buffer);
        }
    }
}
