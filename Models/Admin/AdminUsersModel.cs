using HttpEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogEngine.Models.Admin
{
    internal class AdminUsersModel : AdminModel
    {
        public AdminUsersModel()
        {
            Routes = new()
            {
                "/admin/users"
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

            byte[] buffer = File("admin/users.html", request);

            buffer = buffer.ParseView(new()
            {
                ["title"] = "Users",
            });

            return new ModelResult(buffer);
        }
    }
}
