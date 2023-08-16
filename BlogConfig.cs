using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogEngine.Database;

namespace BlogEngine
{
    internal class BlogConfig
    {
        public static bool IsConfigured
        {
            get => GetProperty("is_configured") != "0" && GetProperty("is_configured") != null;
            set => SetProperty("is_configured", value ? "1" : "0");
        }

        public static string ApplicationName
        {
            get => GetProperty("app_name");
            set => SetProperty("app_name", value);
        }

        public static string ApplicationDescription
        {
            get => GetProperty("app_desc");
            set => SetProperty("app_desc", value);
        }

        public static string Theme
        {
            get => GetProperty("theme_dir");
            set => SetProperty("theme_dir", value);
        }
        public static Page? IndexPage
        {
            get => new BlogContext().Pages.FirstOrDefault(x => x.Id == Convert.ToInt32(GetProperty("main_page_id")));
            set
            {
                if (value != null)
                    SetProperty("main_page_id", value.Id.ToString());
            }
        }

        public static bool AllowComments
        {
            get => GetProperty("allow_comments") != "0" && GetProperty("allow_comments") != null;
            set => SetProperty("allow_comments", value ? "1" : "0");
        }

        public static bool ModerateComments
        {
            get => GetProperty("moderate_comments") != "0" && GetProperty("moderate_comments") != null;
            set => SetProperty("moderate_comments", value ? "1" : "0");
        }

        private static string GetProperty(string name)
        {
            var db = new BlogContext();
            ConfigProperty? property = db.Config.FirstOrDefault(x => x.Name == name);
            if (property == null)
            {
                property = db.Config.Add(new ConfigProperty(name, "0")).Entity;
                db.SaveChanges();
            }
            return property.Value;
        }

        private static void SetProperty(string name, string? value)
        {
            if (value == null)
                return;

            var db = new BlogContext();
            ConfigProperty? property = db.Config.FirstOrDefault(x => x.Name == name);
            if (property == null)
                db.Add(new ConfigProperty(name, value));
            else
                property.Value = value;
            db.SaveChanges();
        }
    }
}
