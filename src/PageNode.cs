namespace TelegramContentLanguage
{
    public class PageNode
    {
        public readonly string[] PathSegments;
        public readonly string Path;

        public Page? Page;
        public Dictionary<string, PageNode> Children;

        public PageNode(
            string[] pathSegments, 
            string path, 
            Page? page, 
            Dictionary<string, PageNode> children)
        {
            PathSegments = pathSegments;
            Path = path;
            Page = page;
            Children = children;
        }

        public IEnumerable<PageNode> GetNotNullChildren()
        {
            foreach (KeyValuePair<string, PageNode> child in Children)
            {
                if (child.Value.Page != null)
                {
                    yield return child.Value;
                }
            }
        }
    }
}
