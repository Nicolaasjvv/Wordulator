using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WordulatorCore.Interfaces;
using WordulatorCore.Models;
using WordulatorCore.Utilities;
using WordulatorCore.Services;

namespace WordulatorCore
{
    public class TextfileProcessor : IProcessor
    {
        public long LastProcessTimeMilli { get; internal set; }

        private string _filePath ;
        private List<WordCountObject> _parsedWordCounts;
        public TextfileProcessor(string path)
        {
            _filePath = path;
            _parsedWordCounts = new List<WordCountObject>();
        }

        public async Task<bool> ParseDocument()
        {
            if (string.IsNullOrEmpty(_filePath))
                throw new FileNotFoundException();

            try
            {
                _parsedWordCounts = new List<WordCountObject>(); //reset list
                //running async so that consuming apps don't freeze up their UI
                Task<List<WordCountObject>> top50Task = Task.Run(() => ParseDocumentWrapper());
                _parsedWordCounts = await top50Task;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return false;
            }
            
        }

        public List<WordCountObject> GetTop50WordOccurrences()
        {
            var myStopWatch = new Stopwatch();
            myStopWatch.Start();
            var returnList = _parsedWordCounts.OrderByDescending(x => x.Occurrences).Take(50).ToList();
            myStopWatch.Stop();
            LastProcessTimeMilli = myStopWatch.ElapsedMilliseconds;
            return returnList;
        }

        public List<WordCountObject> GetTop50WordsBtSix()
        {
            var myStopWatch = new Stopwatch();
            myStopWatch.Start();
            var returnList = _parsedWordCounts.Where(x => x.Word.Length > 6).OrderByDescending(x => x.Occurrences).Take(50).ToList();
            myStopWatch.Stop();
            LastProcessTimeMilli = myStopWatch.ElapsedMilliseconds;
            return returnList;            
        }

        //wrapper method to simplify when using as task. 
        private List<WordCountObject> ParseDocumentWrapper()
        {
            var myStopWatch = new Stopwatch();
            myStopWatch.Start();

            var contents = FileService.GetStringFromFile(_filePath); //get all the string content from a file

            var allWords = Utils.GetWordsFromString(contents);

            var wordCounts = allWords.GroupBy(x => x).Select(x => new WordCountObject(x.Key, x.Count()));

            myStopWatch.Stop();
            LastProcessTimeMilli = myStopWatch.ElapsedMilliseconds;

            return wordCounts.ToList();
        }


    }
}
