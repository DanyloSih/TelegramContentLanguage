using System.Text;
using SimpleContentLanguage;

namespace TelegramContentLanguage
{
    public class PagesContainer
    {
        public readonly PageNode MainPageNode;

        private Dictionary<string, PageNode> _pathNodeAssociations;
        private TCLParsingConfig _parsingConfig;
        private TCLErrorsConfig _errorsConfig;

        public PagesContainer(TCLParsingConfig parsingConfig, TCLErrorsConfig errorsConfig)
        {
            MainPageNode = new PageNode(["main"], "main", null, new(), null);
            _pathNodeAssociations = new() { { MainPageNode.Path, MainPageNode } };
            _parsingConfig = parsingConfig;
            _errorsConfig = errorsConfig;
        }

        public void Clear()
        {
            _pathNodeAssociations.Clear();
            MainPageNode.Children.Clear();
            _pathNodeAssociations.Add(MainPageNode.Path, MainPageNode);
        }

        public IEnumerable<PageNode> IterateHierarchy(PageNode targetNode)
        {
            if (targetNode == null)
            {
                yield break;
            }

            PageNode? node = MainPageNode;

            yield return node;

            for (int i = 0; i < targetNode.PathSegments.Length; i++)
            {
                string segment = targetNode.PathSegments[i];   
                
                if (!node.Children.TryGetValue(segment, out node))
                {
                    throw new InvalidOperationException(
                        $"Hierarchy inconsistency: segment '{segment}' was not found among children of node '{node?.Path}'.");
                }

                yield return node;
            }       
        }

        public Result SetPage(Page page, Token pathToken)
        {
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
            PageNode? parent = MainPageNode;
            for (int i = 0; i < pathSegments.Length; i++)
            {
                string pathSegment = pathSegments[i];

                if (i == lastId)
                {
                    PageNode node = new PageNode(pathSegments, path, parent, new(), page);
                    nodeChildren[pathSegment] = node;
                    _pathNodeAssociations[path] = node;
                }
                else
                {
                    if (!nodeChildren.TryGetValue(pathSegment, out PageNode? child))
                    {
                        child = new PageNode(pathSegments, path, parent, new(), null);
                        nodeChildren[pathSegment] = child;
                        _pathNodeAssociations[path] = child;
                    }

                    parent = child;
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
            if (pathSegments == null 
             || pathSegments.Length == 0 
             || pathSegments[0].Equals(MainPageNode.Path))
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
