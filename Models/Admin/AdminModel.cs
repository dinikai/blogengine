using BlogEngine.Database;
using HttpEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BlogEngine.Models.Admin
{
    internal abstract class AdminModel : Model
    {
        public static bool CheckLogin(ModelRequest request)
        {
            Cookie? id = request.RequestCookies["blog_userid"];
            Cookie? password = request.RequestCookies["blog_userpwd"];
            if (id == null || password == null)
                return false;

            using BlogContext db = new BlogContext();
            try
            {
                User? user = db.Users.FirstOrDefault(x => x.Id == Convert.ToInt32(id.Value) && x.Password == password.Value);
                if (user != null)
                    return true;
            }
            catch
            {
                return false;
            }
            return false;
        }
    }
}
