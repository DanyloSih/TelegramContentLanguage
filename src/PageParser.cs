using SimpleContentLanguage;

namespace TelegramContentLanguage
{
    public class PageParser : IElementParser
    {
        public IElementRecognizer ElementRecognizer { get; private set; }

        public PageParser(BlockRecognizer recognizer)
        {
            ElementRecognizer = recognizer;
        }

        public Result Parse(List<TokenizedLine> tokenizedLines, ElementBounds elementBounds)
        {
            TokenizedLine firstLine = tokenizedLines[0];

            int startTokenId = elementBounds.StartToken.TokenId;
            Token pathToken = firstLine.Tokens[startTokenId + 1];
            Token buttonNameToken = firstLine.Tokens[startTokenId + 2];

            Console.WriteLine($"Path token: {pathToken.Value}; Button name token: {buttonNameToken.Value}");

            return new Result(true, string.Empty);
        }
    }
}
