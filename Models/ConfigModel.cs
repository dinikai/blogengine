using BlogEngine.Database;
using HttpEngine.Core;

namespace BlogEngine.Models
{
    internal class ConfigModel : Model
    {
        public ConfigModel()
        {
            UseLayout = false;
        }

        public override ModelResult OnRequest(ModelRequest request)
        {
            byte[] buffer = File("config/configure.html", request);

            if (request.Method == HttpEngine.Core.HttpMethod.Post)
            {
                try
                {
                    string applicationName = request.Arguments.Arguments["app_name"];
                    string login = request.Arguments.Arguments["adm_login"];
                    string password = request.Arguments.Arguments["adm_password"];

                    BlogConfig.IsConfigured = true;
                    BlogConfig.ApplicationName = applicationName;
                    BlogConfig.ApplicationDescription = "Another blog on blogengine";
                    BlogConfig.Theme = "basic";
                    BlogConfig.AllowComments = true;
                    BlogConfig.ModerateComments = true;

                    using BlogContext db = new();
                    db.Users.Add(new User
                    {
                        UserName = login,
                        Nickname = login,
                        Password = password,
                    });
                    Page mainPage = db.Pages.Add(new Page
                    {
                        Name = "Main",
                        Url = "index",
                        Content = "# This is your blog\nEdit or delete this page and begin adding content\n> [Go to admin panel](/admin)"
                    }).Entity;
                    db.Notes.Add(new()
                    {
                        Title = "Hello",
                        Text = "# Hello",
                        LastChanged = DateTime.Now,
                        RenderType = RenderType.Markdown,
                        NoteStatus = NoteStatus.Private,
                    });

                    db.SaveChanges();

                    BlogConfig.IndexPage = mainPage;

                    RemoveModel();
                    return Redirect("/admin/login");
                }
                catch
                {
                }
            }

            return new ModelResult(buffer);
        }
    }
}
