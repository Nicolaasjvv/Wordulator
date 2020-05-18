using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordulatorCore.Utilities
{
    public static  class Utils
    {        
        public static List<string> GetUniqueWords(string input)
        {
            string delim = " ,.";
            var words = input.Split(delim.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            return words.Distinct().ToList();

        }

        /// <summary>
        /// Optimised method that traverses characters in stead of using a regex split, as the latter is sometimes slower.
        /// </summary>
        /// <param name="content">content for which to get words</param>
        /// <returns>list of words</returns>
        public static List<string> GetWordsFromString(string content)
        {

            // Allocate array.
            var returnWords = new List<string>();

            // Add words to array.
            bool onWord = false;
            var builder = new StringBuilder();

            for (int i = 0; i < content.Length; i++)
            {
                bool wordChar = char.IsLetterOrDigit(content[i]);
                // Append to current word.
                if (wordChar)
                {
                    builder.Append(char.ToLowerInvariant(content[i]));
                }
                // If we are not on a valid char and only if our builder already has content, end and add the word to the list
                if ((!wordChar && builder.Length > 0) || i == content.Length - 1)
                {
                    // Store the word, and clear the builder.
                    returnWords.Add(builder.ToString());
                    builder.Clear();
                }
            }

            return returnWords;
        }



        /// <summary>
        /// Optimised methods that uses arrays to process words. May be used to provide small boost in performance
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static string[] GetWordsFromStringUsingArray(string line)
        {
            // Count words.
            int count = 0;
            bool onWord = false;
            for (int i = 0; i < line.Length; i++)
            {
                // If we are on the first char of a word, increase word count.
                bool wordChar = char.IsLetterOrDigit(line[i]);
                if (wordChar && !onWord)
                {
                    onWord = true;
                    // Add to word.
                    count++;
                }
                // If not on word char, set bool to false.
                if (!wordChar)
                {
                    onWord = false;
                }
            }
            // Allocate array.
            string[] words = new string[count];

            // Add words to array.
            var builder = new StringBuilder();
            int wordIndex = 0;
            for (int i = 0; i < line.Length; i++)
            {
                bool wordChar = char.IsLetterOrDigit(line[i]);
                // Append to current word.
                if (wordChar)
                {
                    builder.Append(line[i]);
                }
                // Else, end and add the word to the list
                if (!wordChar || i == line.Length - 1)
                {


                    // Store the word, and clear the buffer.
                    words[wordIndex++] = builder.ToString();
                    builder.Clear();
                }
            }
            return words;
        }


    }
}
