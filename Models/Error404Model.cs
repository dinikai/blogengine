using HttpEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogEngine.Models
{
    internal class Error404Model : Model
    {
        public Error404Model()
        {
            Routes = new()
            {
                "/note/{id}"
            };
        }

        public override ModelResult OnRequest(ModelRequest request)
        {
            byte[] buffer = File($"themes/{BlogConfig.Theme}/404.html", request);

            buffer = buffer.ParseView(new()
            {
                ["title"] = "Page not found",
                ["url"] = request.Url
            });

            return new ModelResult(buffer);
        }
    }
}
