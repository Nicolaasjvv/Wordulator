using System;
using System.Collections.Generic;
using System.IO;
using WordulatorCore.Utilities;

namespace WordulatorCore.Services
{
    public class FileService
    {
        public static List<string> GetLinesFromFile(string path)
        {
            List<string> lines = new List<string>();

            using (StreamReader sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    if (!string.IsNullOrEmpty(line))
                        lines.Add(line);
                }
            }

            return lines;
        }

        /// <summary>
        /// Currently most easy asnd effective way of retrieving text from file. Downside may be memory issues as all is loaded into memory. 
        /// using streamreader with a buffer or reading lines may ease memory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetStringFromFile(string path)
        {
            var contents = File.ReadAllText(path);
            return contents;
        }

        //alternative method for fetching file content, one line at a time. 
        [Obsolete]
        public static List<string> GetWordsFromFileUsingLines(string path)
        {        
            List<string> words = new List<string>();

            using (StreamReader sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();               

                    if (!string.IsNullOrEmpty(line))
                    {
                        words.AddRange(Utils.GetWordsFromString(line));
                    }
                }
            }

            return words;
        }

       
    }
}
