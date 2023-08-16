using BlogEngine;
using BlogEngine.Database;
using BlogEngine.Models;
using BlogEngine.Models.Admin;
using HttpEngine.Core;

var options = new HttpApplicationBuilderOptions()
{
    Hosts = new string[]
    {
        "http://*:80/"
    },
    Layout = new IndexLayout()
};
var builder = new HttpApplicationBuilder(options);
var app = builder.Build();

app.Use404<Error404Model>();


app.MapGet("/admin/comments/remove", (request) =>
{
    if (!AdminModel.CheckLogin(request))
        return new SkipResult();

    BlogContext db = new();
    try
    {
        Comment? comment = db.Comments.FirstOrDefault(x => x.Id == Convert.ToInt32(request.Arguments.Arguments["id"]));
        if (comment != null)
        {
            db.Comments.Remove(comment);
        }

        db.SaveChanges();
    }
    catch
    {
        return new SkipResult();
    }

    return Model.Redirect(request.Headers["Referer"]!);
});

app.MapGet("/admin/notes/remove", (request) =>
{
    if (!AdminModel.CheckLogin(request))
        return new SkipResult();

    BlogContext db = new();
    try
    {
        Note? note = db.Notes.FirstOrDefault(x => x.Id == Convert.ToInt32(request.Arguments.Arguments["id"]));
        if (note != null)
        {
            db.Notes.Remove(note);

            foreach (var comment in db.Comments)
            {
                if (comment.NoteId == note.Id)
                    db.Comments.Remove(comment);
            }
        }

        db.SaveChanges();
    }
    catch
    {
        return new SkipResult();
    }

    return Model.Redirect(request.Headers["Referer"]!);
});

app.MapGet("/admin/pages/remove", (request) =>
{
    if (!AdminModel.CheckLogin(request))
        return new SkipResult();

    BlogContext db = new();
    try
    {
        Page? page = db.Pages.FirstOrDefault(x => x.Id == Convert.ToInt32(request.Arguments.Arguments["id"]));
        if (page != null)
            db.Pages.Remove(page);
        db.SaveChanges();
    }
    catch
    {
        return new SkipResult();
    }

    return Model.Redirect(request.Headers["Referer"]!);
});

app.Map("/admin/notes/new", (request) =>
{
    if (!AdminModel.CheckLogin(request))
        return new SkipResult();

    BlogContext db = new();

    if (request.Arguments.Arguments.ContainsKey("type"))
    {
        if (request.Arguments.Arguments["type"] == "page")
        {
            Page page = db.Pages.Add(new()
            {
                Name = request.Arguments.Arguments.ContainsKey("name") ? request.Arguments.Arguments["name"] : "New Page",
                Url = "",
                Content = request.Arguments.Arguments.ContainsKey("content") ? request.Arguments.Arguments["content"] : "",
                RenderType = RenderType.Markdown
            }).Entity;

            db.SaveChanges();
            page.Url = page.Id.ToString();
            db.SaveChanges();

            if (request.Arguments.Arguments.ContainsKey("referer"))
                return Model.Redirect(request.Headers["Referer"]!);
            else
                return Model.Redirect($"/admin/pages/edit/?id={page.Id}");
        }
        else
            return new SkipResult();
    }
    else
    {
        NoteStatus noteStatus = NoteStatus.Draft;
        if (request.Arguments.Arguments.ContainsKey("status"))
        {
            try
            {
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
            }
            catch { }
        }
            
        Note note = new()
        {
            Title = request.Arguments.Arguments.ContainsKey("name") ? request.Arguments.Arguments["name"] : "New Note",
            Text = request.Arguments.Arguments.ContainsKey("content") ? request.Arguments.Arguments["content"] : "What do you think?",
            LastChanged = DateTime.UtcNow,
            RenderType = RenderType.Markdown,
            NoteStatus = noteStatus
        };
        db.Notes.Add(note);
        db.SaveChanges();

        if (request.Arguments.Arguments.ContainsKey("referer"))
            return Model.Redirect(request.Headers["Referer"]!);
        else
            return Model.Redirect($"/admin/notes/edit/?id={note.Id}");
    }
});

app.MapGet("/admin/comments/allow", (request) =>
{
    if (!AdminModel.CheckLogin(request))
        return new SkipResult();

    BlogContext db = new();

    try
    {
        db.Comments.First(x => x.Id == Convert.ToInt32(request.Arguments.Arguments["id"])).Status = CommentStatus.Allowed;
        db.SaveChanges();
    } catch
    {
        return new SkipResult();
    }
    return Model.Redirect(request.Headers["Referer"]!);
});

app.MapGet("/admin/comments/block", (request) =>
{
    if (!AdminModel.CheckLogin(request))
        return new SkipResult();

    BlogContext db = new();

    try
    {
        db.Comments.First(x => x.Id == Convert.ToInt32(request.Arguments.Arguments["id"])).Status = CommentStatus.Blocked;
        db.SaveChanges();
    }
    catch
    {
        return new SkipResult();
    }
    return Model.Redirect(request.Headers["Referer"]!);
});

if (!BlogConfig.IsConfigured)
    app.UseModel<ConfigModel>();

app.UseModel<PageModel>();
app.UseModel<IndexModel>();
app.UseModel<NoteModel>();

app.UseModel<AdminConsoleModel>();
app.UseModel<AdminNotesModel>();
app.UseModel<AdminPagesModel>();
app.UseModel<AdminCommentsModel>();
app.UseModel<AdminUsersModel>();
app.UseModel<AdminSettingsModel>();
app.UseModel<AdminEditModel>();
app.UseModel<AdminLoginModel>();

app.Run();