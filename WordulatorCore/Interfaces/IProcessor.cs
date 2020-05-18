using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WordulatorCore.Models;

namespace WordulatorCore.Interfaces
{
    interface IProcessor
    {        
        long LastProcessTimeMilli { get; }
        Task<bool> ParseDocument();
        List<WordCountObject> GetTop50WordOccurrences();
        List<WordCountObject> GetTop50WordsBtSix();

    }
}
