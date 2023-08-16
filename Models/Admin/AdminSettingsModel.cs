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

            buffer = buffer.ParseView(new()
            {
                ["title"] = "Settings",
                ["applicationName"] = BlogConfig.ApplicationName,
                ["applicationDescription"] = BlogConfig.ApplicationDescription,
                ["themeOptions"] = themeOptions,
                ["allowComments"] = BlogConfig.AllowComments ? "checked" : "",
                ["moderateComments"] = BlogConfig.ModerateComments ? "checked" : "",
                ["messages"] = messages
            });

            return new ModelResult(buffer);
        }
    }
}
