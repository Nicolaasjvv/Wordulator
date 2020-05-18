using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Wordulator.Annotations;
using WordulatorCore;
using WordulatorCore.Models;

namespace Wordulator.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _activeFilePath, _friendlyFileName, _mainText, _secondaryText;

        public string MainText
        {
            get => _mainText;
            set => SetProperty(ref _mainText, value);
        }

        public string SecondaryText
        {
            get => _secondaryText;
            set => SetProperty(ref _secondaryText, value);
        }

        public string FriendlyFileName
        {
            get => Path.GetFileName(_friendlyFileName);
            set { _friendlyFileName = value; RaisePropertyChanged(); }
        }


        public string ActiveFilePath
        {
            get => _activeFilePath;
            set { _activeFilePath = FriendlyFileName = value; }
        }


        /// <summary>
        /// This method can be refactored into controller endpoint in case of moving to service / cloud service, changing it to accept intput rather than using an already set file
        /// </summary>
        public async void CountWords()
        {
            if (string.IsNullOrEmpty(ActiveFilePath))
            {                
                MainText = "No file chosen";
                return;
            }

            try
            {
                List<WordCountObject> wordCounts = new List<WordCountObject>(), wordCountsBt6 = new List<WordCountObject>();
                long executionTime = 0;
                if (Path.GetExtension(ActiveFilePath) == ".txt")
                {
                    TextfileProcessor txtProc = new TextfileProcessor(ActiveFilePath);
                    if (await txtProc.ParseDocument())
                    {
                        executionTime = txtProc.LastProcessTimeMilli;
                        wordCounts = txtProc.GetTop50WordOccurrences();
                        executionTime += txtProc.LastProcessTimeMilli;
                        wordCountsBt6 = txtProc.GetTop50WordsBtSix();
                        executionTime += txtProc.LastProcessTimeMilli;
                    }
                    else
                        MainText = "Can not parse document";
                }
                else if (Path.GetExtension(ActiveFilePath) == ".epub")
                {
                    MainText = SecondaryText = "Epub can not be rendered at this time for processing. For demo please see code method \"CountWords\" ";

                    //epub processing currently has an issue where the webview needed to render and retrieve text is not loading the provided html. 
                    //to see the parsing of the epub in action, comment out the code below and follow the code during debugging, starting with method GetTop50WordOccurrences()

                    //EpubProcessor epubProc = new EpubProcessor(ActiveFilePath);
                    //if (await epubProc.ParseDocument())
                    //{
                    //    executionTime = epubProc.LastProcessTimeMilli;
                    //    wordCounts = epubProc.GetTop50WordOccurrences();
                    //    executionTime += epubProc.LastProcessTimeMilli;
                    //    wordCountsBt6 = epubProc.GetTop50WordsBtSix();
                    //    executionTime += epubProc.LastProcessTimeMilli;
                    //}
                    //else
                    //    MainText = "Can not parse document";
                }

                if(wordCounts.Any())
                    WriteProcessResultsToMain(wordCounts.Select(x => ValueTuple.Create(x.Word, x.Occurrences)).ToList(),
                        executionTime);

                if(wordCountsBt6.Any())
                    WriteProcessResultsToSecondary(wordCountsBt6.Select(x => ValueTuple.Create(x.Word, x.Occurrences)).ToList(),
                        executionTime);                
            }
            catch (Exception ex)
            {
                MainText = "An error occurred during processing. Please contact support";
            }
        }



        /// <summary>
        /// Function to handle formatting and writing of word results to the main output. Using valueTuple decouples from unnecessary dependance on type WordCountObject
        /// </summary>
        private void WriteProcessResultsToMain(List<(string word, int count)> topWords, long executionTime)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Book Processed in: {executionTime} milliseconds");
            sb.AppendLine("");
            sb.AppendLine("(occurrence)  :  word");
            sb.AppendLine("--");
            topWords.ForEach(x => sb.AppendLine($"({x.count})  :  {x.word}"));
            MainText = sb.ToString();
        }

        /// <summary>
        /// Function to handle formatting and writing of word results to the main output. Using valueTuple decouples from unnecessary dependance on type WordCountObject
        /// </summary>
        private void WriteProcessResultsToSecondary(List<(string word, int count)> topWords, long executionTime)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Book Processed in: {executionTime} milliseconds");
            sb.AppendLine("");
            sb.AppendLine("(occurrence)  :  word");
            sb.AppendLine("--");
            topWords.ForEach(x => sb.AppendLine($"({x.count})  :  {x.word}"));
            SecondaryText = sb.ToString();
        }

        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Method to enable Shorthand setter so that we don't have to call "RaisePropertyChanged" each time we set a value
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string property
            = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            RaisePropertyChanged(property);
            return true;
        }

        #endregion
    }
}
