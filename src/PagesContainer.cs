using SimpleContentLanguage;

namespace TelegramContentLanguage
{
    public class PagesContainer
    {
        public readonly PageNode MainPageNode;

        private readonly char _pathSeparator;
        private readonly string _emptyPathSegmentErrorMessageFormat;

        /// <param name="pathSeparator">Path with ' / ' separator: a/b/c </param>
        /// <param name="emptyPathSegmentErrorMessageFormat">{0} - page full path</param>
        public PagesContainer(char pathSeparator, string emptyPathSegmentErrorMessageFormat)
        {
            _pathSeparator = pathSeparator;
            _emptyPathSegmentErrorMessageFormat = emptyPathSegmentErrorMessageFormat;

            MainPageNode = new PageNode(new(string.Empty, string.Empty, string.Empty), new());
        }

        public Result SetPage(Page page)
        {
            string[] path = page.Path.Split(_pathSeparator);
            int lastId = path.Length - 1;

            for (int i = 0; i < path.Length; i++)
            {
                if (string.IsNullOrEmpty(path[i]))
                {
                    return new Result(
                        false,
                        string.Format(_emptyPathSegmentErrorMessageFormat, page.Path));
                }
            }

            Dictionary<string, PageNode> nodeChildren = MainPageNode.Children;

            for (int i = 0; i < path.Length; i++)
            {
                string pathSegment = path[i];

                if (i == lastId)
                {
                    nodeChildren[pathSegment] = new PageNode(page, new());
                }
                else
                {
                    if (!nodeChildren.TryGetValue(pathSegment, out PageNode? child))
                    {
                        child = new PageNode(null, new());
                        nodeChildren[pathSegment] = child;
                    }

                    nodeChildren = child.Children;
                }
            }

            return new Result(true, string.Empty);
        }

        public bool TryGetPageNode(string path, out PageNode? node)
        {
            if (string.IsNullOrEmpty(path))
            {
                node = MainPageNode;
                return true;
            }

            string[] pathSegments = path.Split(_pathSeparator);
            node = MainPageNode;
            for (int i = 0; i < path.Length; i++)
            {
                string pathSegement = pathSegments[i];

                if (!node.Children.TryGetValue(pathSegement, out node))
                {
                    node = null;
                    return false;
                }
            }

            return true;
        }
    }
}
