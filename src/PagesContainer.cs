using System.Text;
using System.Xml.Linq;
using SimpleContentLanguage;

namespace TelegramContentLanguage
{
    public class PagesContainer
    {
        public PageNode? MainPageNode;

        private Dictionary<string, PageNode> _pathNodeAssociations;
        private TCLParsingConfig _parsingConfig;
        private TCLErrorsConfig _errorsConfig;

        public PagesContainer(TCLParsingConfig parsingConfig, TCLErrorsConfig errorsConfig)
        {
            MainPageNode = null;

            _pathNodeAssociations = new();
            _parsingConfig = parsingConfig;
            _errorsConfig = errorsConfig;
        }

        public void CheckMainPageNodeInitialization()
        {
            if (MainPageNode == null || MainPageNode.Page == null)
            {
                throw new InvalidOperationException($"Before using \"{nameof(PagesContainer)}\", " +
                    $"you should manualy initialize field \"{nameof(MainPageNode)}\" " +
                    $"by node with not null Page value!");
            }
        }

        public IEnumerable<PageNode> IterateHierarchy(PageNode targetNode)
        {
            CheckMainPageNodeInitialization();

            if (targetNode == null)
            {
                yield break;
            }

            if (targetNode == MainPageNode)
            {
                yield return MainPageNode;
            }

            //targetNode.Page.Path.Text.Split();
        }

        public Result SetPage(Page page, Token pathToken)
        {
            CheckMainPageNodeInitialization();

            string path = pathToken.Text;
            string[] pathSegments = path.Split(_parsingConfig.PathSeparator);
            int lastId = pathSegments.Length - 1;

            for (int i = 0; i < pathSegments.Length; i++)
            {
                if (string.IsNullOrEmpty(pathSegments[i]))
                {
                    return new Result(
                        false,
                        _errorsConfig.GetEmptyPathSegmentError(
                           pathToken.SourceLineId + 1, 
                           pathToken.FirstCharPositionInSourceLine,
                           pathToken.Text));
                }
            }

            Dictionary<string, PageNode> nodeChildren = MainPageNode!.Children;

            for (int i = 0; i < pathSegments.Length; i++)
            {
                string pathSegment = pathSegments[i];

                if (i == lastId)
                {
                    PageNode node = new PageNode(pathSegments, path, page, new());
                    nodeChildren[pathSegment] = node;
                    _pathNodeAssociations[path] = node;
                }
                else
                {
                    if (!nodeChildren.TryGetValue(pathSegment, out PageNode? child))
                    {
                        child = new PageNode(pathSegments, path, null, new());
                        nodeChildren[pathSegment] = child;
                        _pathNodeAssociations[path] = child;
                    }

                    nodeChildren = child.Children;
                }
            }

            return new Result(true, string.Empty);
        }

        public bool TryGetPageNode(string path, out PageNode? node)
        {
            return _pathNodeAssociations.TryGetValue(path, out node);
        }

        public bool TryGetPageNode(string[] pathSegments, out PageNode? node)
        {
            CheckMainPageNodeInitialization();

            if (pathSegments == null)
            {
                node = MainPageNode!;
                return true;
            }

            node = MainPageNode!;

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

        public override string ToString()
        {
            CheckMainPageNodeInitialization();
            StringBuilder sb = new StringBuilder();

            string rootName = MainPageNode!.Page!.Name;

            if (string.IsNullOrEmpty(rootName))
            {
                rootName = "<root>";
            }

            sb.AppendLine(rootName);
            AppendNodeChildren(sb, MainPageNode!, string.Empty);

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
