using HttpEngine.Core;

namespace BlogEngine.Models
{
    internal class IndexLayout : Layout
    {
        public IndexLayout(string publicDirectory) : base(publicDirectory)
        {
        }

        public IndexLayout()
        {
        }

        public override byte[] OnRequest(ModelRequest request)
        {
            byte[] buffer = File($"themes/{BlogConfig.Theme}/_Layout.html");

            buffer = buffer.ParseView(new()
            {
                ["applicationName"] = BlogConfig.ApplicationName,
                ["applicationDescription"] = BlogConfig.ApplicationDescription
            });

            return buffer;
        }
    }
}
