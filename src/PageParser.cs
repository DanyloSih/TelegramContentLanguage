using System.Text;
using SimpleContentLanguage;

namespace TelegramContentLanguage
{
    public class PageParser : IElementParser
    {
        private readonly PagesContainer _pagesContainer;
        private readonly BlockRecognizer _recognizer;
        private readonly string _incorrectArgumentsCountErrorMessageFormat;

        public IElementRecognizer ElementRecognizer { get => _recognizer; }

        /// <param name="recognizer"></param>
        /// <param name="pagesContainer"></param>
        /// <param name="incorrectArgumentsCountErrorMessageFormat">
        /// {0} - Element type; {1} - Expected arguments count; 
        /// {2} - Start token line id (row); {3} - Start token position (column)</param>
        public PageParser(
            BlockRecognizer recognizer,
            PagesContainer pagesContainer,
            string incorrectArgumentsCountErrorMessageFormat)
        {
            _pagesContainer = pagesContainer;
            _recognizer = recognizer;
            _incorrectArgumentsCountErrorMessageFormat = incorrectArgumentsCountErrorMessageFormat;
        }

        public Result Parse(TokenizedBlock tokenizedBlock, TokenBounds elementBounds)
        {
            if (!tokenizedBlock.TryGetNextTokenInBounds(elementBounds.StartToken, elementBounds, out Token pathToken)
             || !tokenizedBlock.TryGetNextTokenInBounds(pathToken, elementBounds, out Token buttonNameToken)
             || buttonNameToken.Value.Equals(elementBounds.EndToken.Value))
            {
                return new Result(false, string.Format(
                    _incorrectArgumentsCountErrorMessageFormat, 
                    _recognizer.StartToken,
                    2,
                    elementBounds.StartToken.SourceLineId + 1,
                    elementBounds.StartToken.FirstCharPositionInSourceLine));
            }

            
            string content = string.Empty;

            if (tokenizedBlock.TryGetNextTokenInBounds(buttonNameToken, elementBounds, out Token contentStart)
             && tokenizedBlock.TryGetPreviousTokenInBounds(elementBounds.EndToken, elementBounds, out Token contentEnd))
            {
                content = tokenizedBlock.CreateMergedMetaLinesInBounds(new TokenBounds(contentStart, contentEnd));
            }

            _pagesContainer.SetPage(new Page(pathToken.Value, buttonNameToken.Value, content.ToString()));

            return new Result(true, string.Empty);
        }
    }
}
