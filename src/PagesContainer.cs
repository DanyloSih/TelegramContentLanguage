using System.Text;
using SimpleContentLanguage;

namespace TelegramContentLanguage
{
    public class PagesContainer
    {
        public readonly PageNode MainPageNode;

        private TCLParsingConfig _parsingConfig;
        private TCLErrorsConfig _errorsConfig;

        public PagesContainer(TCLMebmersConfig mebmersConfig, TCLParsingConfig parsingConfig, TCLErrorsConfig errorsConfig)
        {
            Token path = default;
            path.Text = string.Empty;
            MainPageNode = new PageNode(new(path, mebmersConfig.MainPageName, string.Empty), new());

            _parsingConfig = parsingConfig;
            _errorsConfig = errorsConfig;
        }

        public Result SetPage(Page page)
        {
            string[] path = page.Path.Text.Split(_parsingConfig.PathSeparator);
            int lastId = path.Length - 1;

            for (int i = 0; i < path.Length; i++)
            {
                if (string.IsNullOrEmpty(path[i]))
                {
                    return new Result(
                        false,
                        _errorsConfig.GetEmptyPathSegmentError(
                            page.Path.SourceLineId + 1, 
                            page.Path.FirstCharPositionInSourceLine, 
                            page.Path.Text));
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

            string[] pathSegments = path.Split(_parsingConfig.PathSeparator);
            node = MainPageNode;

            for (int i = 0; i < pathSegments.Length; i++)
            {
                string pathSegment = pathSegments[i];

                if (!node.Children.TryGetValue(pathSegment, out node))
                {
                    node = null;
                    return false;
                }
            }

            return true;
        }

        public void SetMainConfigContent(string content)
        {
            MainPageNode.Page!.Content = content;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            string rootName = MainPageNode.Page?.Name ?? string.Empty;

            if (string.IsNullOrEmpty(rootName))
            {
                rootName = "<root>";
            }

            sb.AppendLine(rootName);
            AppendNodeChildren(sb, MainPageNode, string.Empty);

            return sb.ToString();
        }

        private static void AppendNodeChildren(StringBuilder sb, PageNode node, string indent)
        {
            List<KeyValuePair<string, PageNode>> children = node.Children
                .OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
                .ToList();

            for (int i = 0; i < children.Count; i++)
            {
                KeyValuePair<string, PageNode> pair = children[i];
                string pathSegment = pair.Key;
                PageNode child = pair.Value;

                bool isLast = i == children.Count - 1;
                string branch = isLast ? "└── " : "├── ";

                string displayName = child.Page?.Name ?? pathSegment;

                sb.Append(indent);
                sb.Append(branch);
                sb.AppendLine(displayName);

                string nextIndent = indent + (isLast ? "    " : "│   ");
                AppendNodeChildren(sb, child, nextIndent);
            }
        }
    }
}
