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
        public TextfileProcessor(string path)
        {
            _filePath = path;
        }


        public async Task<List<WordCountObject>> GetTop50WordOccurrences()
        {
            if(string.IsNullOrEmpty(_filePath))
                throw new FileNotFoundException();

            //running async so that consuming apps don't freeze up their UI
            Task<List<WordCountObject>> top50Task = Task.Run(() => GetTop50Wrapper());
            return await top50Task;

        }
        //wrapper method to simplify when using as task. 
        private List<WordCountObject> GetTop50Wrapper()
        {
            var myStopWatch = new Stopwatch();
            myStopWatch.Start();

            var contents = FileService.GetStringFromFile(_filePath); //get all the string content from a file

            var allWords = Utils.GetWordsFromString(contents);

            var wordCounts = allWords.GroupBy(x => x).Select(x => new WordCountObject(x.Key, x.Count()));


            var topWords = wordCounts.Where(x => x.Word.Length > 6).OrderByDescending(x => x.Occurrences).Take(50);

            myStopWatch.Stop();
            LastProcessTimeMilli = myStopWatch.ElapsedMilliseconds;

            return topWords.ToList();
        }


    }
}
