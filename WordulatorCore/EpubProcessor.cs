using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WordulatorCore.Interfaces;
using WordulatorCore.Models;
using WordulatorCore.Services;
using WordulatorCore.Utilities;

namespace WordulatorCore
{
    public class EpubProcessor : IProcessor
    {
        private string _filePath;
        private List<string> _epubFilePaths = new List<string>();
        private int _filePathCounter = 0, _fileCount = 0;
        private BrowserService _browserService = new BrowserService();
        private EpubService _epubService;
        private List<string> _sessionWords = new List<string>();
        private bool _busyProcessing = false;
        private const int TimeOut = 30000;//timeout for processing to avoid endless situation. Currently 30 seconds

        public long LastProcessTimeMilli { get; internal set; }

        public EpubProcessor(string filePath)
        {
            _filePath = filePath;
            _epubService = new EpubService(_filePath);
            _browserService.PageProcessed += HandleBrowserPageProcessed;
        }

        public async Task<List<WordCountObject>> GetTop50WordOccurrences()
        {
            if (string.IsNullOrEmpty(_filePath))
                throw new FileNotFoundException();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            _epubFilePaths = _epubService.GetPagesFromSpine(); //parse the epub and retrieve a list of all the sections / pages in the book.
                                                               //Each page will then be processed to extract words and count them. For an epub, spine refers to an outline of a book, the same as the spine of a book.

            StartProcessingBook();
            
            while (_busyProcessing && stopWatch.ElapsedMilliseconds < TimeOut) //spin while we wait for web process to finish, don't run over timeout
            {
                await Task.Delay(2000);
            }

            var wordCounts = _sessionWords.GroupBy(x => x).Select(x => new WordCountObject(x.Key, x.Count()));
            var topWords = wordCounts.Where(x => x.Word.Length > 6).OrderByDescending(x => x.Occurrences).Take(50);

            stopWatch.Stop();
            LastProcessTimeMilli = stopWatch.ElapsedMilliseconds;

            return topWords.ToList();
        }


        //because we have to wait for events from a browser, we implement a scheme where we wait for responses, keep track of them, and initiate each in turn.
        private void StartProcessingBook()
        {
            if (_busyProcessing)
                return;

            _filePathCounter = 0;
            if (_epubFilePaths.Any())
            {
                _busyProcessing = true;
                GetAndLoadNextPage();
            }
        }

        /// <summary>
        /// Cycle through the pages/sections listed in the spine, loading each into a webview which in turns returns the rendered text of the page / section
        /// </summary>
        private void GetAndLoadNextPage()
        {
            if (_filePathCounter == _epubFilePaths.Count) //reached the page limit, exit processing state
            {
                _busyProcessing = false;
                return;
            }

            string pageContent = _epubService.GetPageAsString(_epubFilePaths[_filePathCounter++]); //get the string content of the page / section (raw HTML)
            _browserService.LoadPage(pageContent); //load the html into a webview. The webview will raise an event once it's loaded and parsed.
        }

        private void HandleBrowserPageProcessed(string content)
        {
            _sessionWords.AddRange(Utils.GetWordsFromString(content));

            GetAndLoadNextPage();


        }
    }
}
