using HttpEngine.Core;

namespace BlogEngine.Models
{
    internal class PageModel : Model
    {
        public PageModel()
        {
            Routes = new()
            {
                "/{name}"
            };
        }

        public override ModelResult OnRequest(ModelRequest request)
        {
            using BlogContext db = new();
            if (!request.Arguments.Arguments.ContainsKey("name"))
                return Skip();

            Page? page = db.Pages.FirstOrDefault(x => x.Url == request.Arguments.Arguments["name"]);
            if (page == null)
                return Skip();

            byte[] buffer = File($"themes/{BlogConfig.Theme}/page.html", request);

            string content = "invalid content type";
            switch (page.RenderType)
            {
                case RenderType.Markdown:
                    content = Markdown.ToHtml(page.Content);
                    break;
                case RenderType.Html:
                    content = page.Content;
                    break;
            }

            buffer = buffer.ParseView(new()
            {
                ["title"] = page.Name,
                ["name"] = page.Name,
                ["content"] = content,
            });
            return new(buffer);
        }
    }
}
