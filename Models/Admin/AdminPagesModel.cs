using BlogEngine.Database;
using HttpEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogEngine.Models.Admin
{
    internal class AdminPagesModel : AdminModel
    {
        public AdminPagesModel()
        {
            Routes = new()
            {
                "/admin/pages",
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

            byte[] buffer = File("admin/pages/index.html", request);

            var db = new BlogContext();

            string messages = "";
            if (request.Handler == "save")
            {
                try
                {
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

            string pagesList = "";
            {
                int i = 0;
                foreach (Page page in db.Pages)
                {
                    string markup = "Markdown";
                    switch (page.RenderType)
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
                    pagesList += buffer.GetSection("page", new()
                    {
                        ["number"] = ++i,
                        ["id"] = page.Id,
                        ["title"] = page.Name,
                        ["markup"] = markup,
                        ["url"] = page.Url
                    });
                }
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
                ["title"] = "Pages",
                ["messages"] = messages,
                ["pageSelect"] = pageSelect,
                ["pagesCount"] = db.Pages.Count(),
                ["pagesList"] = pagesList,
            });

            return new ModelResult(buffer);
        }
    }
}
