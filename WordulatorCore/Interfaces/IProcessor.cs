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
        Task<List<WordCountObject>> GetTop50WordOccurrences();

    }
}
