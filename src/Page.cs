namespace TelegramContentLanguage
{
    public class Page
    {
        public string Name;
        public string Content;

        public Page(
            string name = "",
            string content = "")
        {
            Name = name;
            Content = content;
        }
    }
}
