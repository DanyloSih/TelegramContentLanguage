namespace TelegramContentLanguage
{
    public class PageNode
    {
        public Page? Page;
        public Dictionary<string, PageNode> Children;

        public PageNode(Page? page, Dictionary<string, PageNode> children)
        {
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
