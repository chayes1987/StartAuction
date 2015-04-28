namespace Auction.Utils
{
    public class MessageParser
    {
        /// <summary>
        /// Parse Message
        /// </summary>
        /// <param name="message">The String to be parsed</param>
        /// <param name="startTag">The starting delimiter</param>
        /// <param name="endTag">The ending delimiter</param>
        /// <returns>The required string</returns>
        public static string parseMessage(string message, string startTag, string endTag)
        {
            int startIndex = message.IndexOf(startTag) + startTag.Length;
            string substring = message.Substring(startIndex);
            return substring.Substring(0, substring.LastIndexOf(endTag));
        }
    }
}
