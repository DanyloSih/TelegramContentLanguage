namespace TelegramContentLanguage
{
    public class Page
    {
        public readonly string Path;

        public string Name;
        public string Content;

        public Page(string path, string name, string content)
        {
            Path = path;
            Name = name;
            Content = content;
        }
    }
}
