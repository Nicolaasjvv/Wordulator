namespace WordulatorCore.Models
{
    public class WordCountObject
    {
        //A note on this type, in this project:
        /*
        * Using this objects allows us to expose it to classes consuming this library. 
        * if we wish to refactor this into a mircroservice or service, the type might not be necessary any more as it's use in this class is not that frequent
        * in such a case, we can just simply use ValueTuples which allows for easy handling and casting.
        */

        public string Word { get; set; }
        public int Occurrences { get; set; }

        public WordCountObject(string word, int count)
        {
            Word = word;
            Occurrences = count;
        }
    }
}
