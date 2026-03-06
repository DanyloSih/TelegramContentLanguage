using SimpleContentLanguage;

namespace TelegramContentLanguage
{
    public class Page
    {
        public readonly Token Path;

        public string Name;
        public string Content;

        public Page(Token path, string name, string content)
        {
            Path = path;
            Name = name;
            Content = content;
        }
    }
}
