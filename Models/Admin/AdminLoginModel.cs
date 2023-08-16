using BlogEngine.Database;
using HttpEngine.Core;
using System.Net;

namespace BlogEngine.Models.Admin
{
    internal class AdminLoginModel : Model
    {
        public AdminLoginModel()
        {
            Routes = new()
            {
                "/admin/login",
            };
            UseLayout = false;
        }

        public override ModelResult OnRequest(ModelRequest request)
        {
            byte[] buffer = File("admin/login.html", request);

            if (request.Method == HttpEngine.Core.HttpMethod.Post)
            {
                using BlogContext db = new BlogContext();

                string login = request.Arguments.Arguments["login"];
                string password = request.Arguments.Arguments["password"];
                if (login != null && password != null)
                {
                    User? user = db.Users.FirstOrDefault(x => x.UserName == login && x.Password == password);
                    if (user != null)
                    {
                        LoginManager.Login(request, user);
                        return Redirect("/admin");
                    }
                }
            }

            buffer = buffer.ParseView(new()
            {
                ["title"] = "Login",
            });

            return new ModelResult(buffer);
        }
    }
}
