using HttpEngine.Core;
using System.Net;

namespace BlogEngine
{
    internal class LoginManager
    {
        public static void Login(ModelRequest request, User user)
        {
            if (request.RequestCookies.FirstOrDefault(x => x.Name == "blog_userid") == null
                && request.RequestCookies.FirstOrDefault(x => x.Name == "blog_userpwd") == null)
            {
                DateTime expires = DateTime.Now.AddMonths(1);
                request.ResponseCookies.Add(new CookieCollection()
                {
                    new Cookie("blog_userid", user.Id.ToString()),
                    new Cookie("blog_userpwd", user.Password) { Expires = expires }
                });
            }
        }
    }
}
