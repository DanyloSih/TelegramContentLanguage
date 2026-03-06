using SimpleContentLanguage;

namespace TelegramContentLanguage
{
    public class TCLErrorsConfig : ErrorsConfig
    {
        /// <summary>
        /// {0} - Line id, {1} - Char id, {2} - Path 
        /// </summary>
        public string EmptyPathSegmentErrorFormat
            = "({0}:{1}) Шлях \"{2}\" містить порожній сегмент. Переконайтеся, що між розділювачами " +
            "шляху немає порожніх частин (наприклад, \"//\").";

        public string GetEmptyPathSegmentError(int lineId, int charId, string path)
        {
            return string.Format(EmptyPathSegmentErrorFormat, lineId, charId, path);
        }
    }
}
