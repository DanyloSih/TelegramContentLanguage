namespace TelegramContentLanguage
{
    public class PageNode
    {
        public readonly string[] PathSegments;
        public readonly string Path;
        public readonly PageNode? Parent;
        public readonly Dictionary<string, PageNode> Children;

        public Page? Page;

        public PageNode(
            string[] pathSegments, 
            string path,
            PageNode? parent,
            Dictionary<string, PageNode> children,
            Page? page)
        {
            PathSegments = pathSegments;
            Path = path;
            Parent = parent;
            Children = children;
            Page = page;
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

        public void AddNamePathToContentRecursively(INamePathBuilder namePathBuilder)
        {
            AddNamePathToContentRecursively(namePathBuilder, new List<string>());
        }

        private void AddNamePathToContentRecursively(
            INamePathBuilder namePathBuilder,
            List<string> currentPath)
        {
            string segmentName;

            if (Page != null && !string.IsNullOrEmpty(Page.Name))
            {
                segmentName = Page.Name;
            }
            else if (PathSegments.Length > 0)
            {
                segmentName = PathSegments[^1];
            }
            else
            {
                segmentName = string.Empty;
            }

            currentPath.Add(segmentName);

            if (Page != null)
            {
                var path = namePathBuilder.Build(currentPath);

                if (!string.IsNullOrEmpty(path))
                {
                    Page.Content = $"{path}\n{Page.Content}";
                }
            }

            foreach (var child in Children.Values)
            {
                child.AddNamePathToContentRecursively(namePathBuilder, currentPath);
            }

            currentPath.RemoveAt(currentPath.Count - 1);
        }
    }
}
