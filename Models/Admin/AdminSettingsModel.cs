using BlogEngine.Database;
using HttpEngine.Core;

namespace BlogEngine.Models.Admin
{
    internal class AdminSettingsModel : AdminModel
    {
        public AdminSettingsModel()
        {
            Routes = new()
            {
                "/admin/settings"
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

            byte[] buffer = File("admin/settings.html", request);

            using BlogContext db = new();

            string messages = "";
            if (request.Handler == "save")
            {
                try
                {
                    BlogConfig.ApplicationName = request.Arguments.Arguments["app_name"];
                    BlogConfig.ApplicationDescription = request.Arguments.Arguments["app_description"];
                    BlogConfig.Theme = request.Arguments.Arguments["theme"];

                    BlogConfig.AllowComments = request.Arguments.Arguments.ContainsKey("allow_comments");
                    BlogConfig.ModerateComments = request.Arguments.Arguments.ContainsKey("moderate_comments");

                    Page? page = db.Pages.FirstOrDefault(x => x.Id == Convert.ToInt32(request.Arguments.Arguments["main_page"]));
                    if (page != null)
                        BlogConfig.IndexPage = page;

                    messages += buffer.GetSection("saveMessage");
                }
                catch
                {
                    messages += buffer.GetSection("errorMessage");
                }
            }

            string themeOptions = "";
            string theme = BlogConfig.Theme;
            foreach (string directory in Directory.GetDirectories($"{PublicDirectory}/themes"))
            {
                string name = new DirectoryInfo(directory).Name;
                themeOptions += buffer.GetSection("themeOption", new()
                {
                    ["dir"] = name,
                    ["name"] = name,
                    ["selected"] = name == theme ? "selected" : ""
                });
            }

            string pageSelect = "";
            Page? mainPage = BlogConfig.IndexPage;
            foreach (Page page in db.Pages)
            {
                string selected = "";
                if (mainPage != null)
                    if (mainPage.Id == page.Id)
                        selected = "selected";
                pageSelect += buffer.GetSection("optionPage", new()
                {
                    ["id"] = page.Id,
                    ["name"] = page.Name,
                    ["url"] = page.Url,
                    ["selected"] = selected
                });
            }

            buffer = buffer.ParseView(new()
            {
                ["title"] = "Settings",
                ["applicationName"] = BlogConfig.ApplicationName,
                ["applicationDescription"] = BlogConfig.ApplicationDescription,
                ["themeOptions"] = themeOptions,
                ["allowComments"] = BlogConfig.AllowComments ? "checked" : "",
                ["moderateComments"] = BlogConfig.ModerateComments ? "checked" : "",
                ["messages"] = messages,
                ["pageSelect"] = pageSelect
            });

            return new ModelResult(buffer);
        }
    }
}
