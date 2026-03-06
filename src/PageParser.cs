using SimpleContentLanguage;

namespace TelegramContentLanguage
{
    public class PageParser : ElementParser
    {
        private PagesContainer _pagesContainer;
        private BlockRecognizer _recognizer;
        private TCLErrorsConfig _tclErrorsConfig;

        public PageParser(
            PagesContainer pagesContainer,
            BlockRecognizer recognizer,
            TCLErrorsConfig tclErrorsConfig) : base(recognizer, 2, tclErrorsConfig)
        {
            _pagesContainer = pagesContainer;
            _recognizer = recognizer;
            _tclErrorsConfig = tclErrorsConfig;
        }

        protected override Result OnParse(Token[] args, TokenizedBlock tokenizedBlock, TokenBounds elementBounds)
        {           
            string content = string.Empty;

            if (tokenizedBlock.TryGetNextTokenInBounds(args[1], elementBounds, out Token contentStart)
             && tokenizedBlock.TryGetPreviousTokenInBounds(elementBounds.EndToken, elementBounds, out Token contentEnd))
            {
                content = tokenizedBlock.CreateMergedMetaLinesInBounds(new TokenBounds(contentStart, contentEnd));
            }

            _pagesContainer.SetPage(new Page(args[0], args[1].Text, content.ToString()));

            return new Result(true, string.Empty);
        }
    }
}
