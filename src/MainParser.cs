using SimpleContentLanguage;

namespace TelegramContentLanguage
{
    public class MainParser : ElementParser
    {
        private PagesContainer _pagesContainer;
        private BlockRecognizer _recognizer;
        private TCLErrorsConfig _tclErrorsConfig;

        public MainParser(
            PagesContainer pagesContainer,
            BlockRecognizer recognizer,
            TCLErrorsConfig tclErrorsConfig) : base(recognizer, 1, tclErrorsConfig)
        {
            _pagesContainer = pagesContainer;
            _recognizer = recognizer;
            _tclErrorsConfig = tclErrorsConfig;
        }

        protected override Result OnParse(Token[] args, TokenizedBlock tokenizedBlock, TokenBounds elementBounds)
        {
            string content = string.Empty;

            if (tokenizedBlock.TryGetNextTokenInBounds(args[0], elementBounds, out Token contentStart)
             && tokenizedBlock.TryGetPreviousTokenInBounds(elementBounds.EndToken, elementBounds, out Token contentEnd))
            {
                content = tokenizedBlock.CreateMergedMetaLinesInBounds(new TokenBounds(contentStart, contentEnd));
            }

            _pagesContainer.MainPageNode = new PageNode([""], "", new Page(args[0].Text, content), new());

            return new Result(true, string.Empty);
        }
    }
}
