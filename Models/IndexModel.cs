using BlogEngine.Database;
using HttpEngine.Core;

namespace BlogEngine.Models
{
    internal class IndexModel : Model
    {
        public IndexModel()
        {
            Routes = new()
            {
                "/"
            };
        }

        public override ModelResult OnRequest(ModelRequest request)
        {
            byte[] buffer = File($"themes/{BlogConfig.Theme}/index.html", request);

            using BlogContext db = new BlogContext();

            Page? indexPage = BlogConfig.IndexPage;
            if (indexPage == null)
                return Skip();

            string content = "invalid content type";
            switch (indexPage.RenderType)
            {
                case RenderType.Markdown:
                    content = Markdown.ToHtml(indexPage.Content);
                    break;
                case RenderType.Html:
                    content = indexPage.Content;
                    break;
            }

            buffer = buffer.ParseView(new()
            {
                ["title"] = indexPage.Name,
                ["name"] = indexPage.Name,
                ["content"] = content,
            });

            return new ModelResult(buffer);
        }
    }
}
