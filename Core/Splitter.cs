using FormatWith;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Core
{
    /// <summary>
    /// Splits a single long message into a series of Tweets 
    /// </summary>
    public class Splitter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Splitter"/> class.
        /// </summary>
        /// <param name="splitterConfiguration">The splitter configuration.</param>
        /// <exception cref="NotImplementedException"></exception>
        /// 

        public int  _MaxTweetLength, _ShortUrlCharLength;
        public string mentionListTostring, continuation, continues, message;
        public string _TweetFormat, _ContinuationText, _ContinuesText, _UrlRegex; 
            
        public Splitter(SplitterConfiguration splitterConfiguration)
        {
            _MaxTweetLength = splitterConfiguration.MaximumTweetLength;
            _ShortUrlCharLength = splitterConfiguration.ShortenedUrlCharacterLength;
            _TweetFormat = splitterConfiguration.TweetFormat;
            _ContinuationText = splitterConfiguration.ContinuationText;
            _ContinuesText = splitterConfiguration.ContinuesText;
            _UrlRegex = splitterConfiguration.UrlRegex;

        }

        /// <summary>
        /// Splits the specified message into a series of tweets.
        /// </summary>
        /// <param name="message">The message to be split.</param>
        /// <returns>A series of tweets to be posted to Twitter, in the order in which they should be posted.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<ITweet> Split(string message)
        {
            //create a string arring
            string[] words = message.Split(' ');
            //call a function to split words uisng MaxtTweetLength 
            List<string> splettedMessageTweets = TwitterSplit(words,_MaxTweetLength);
            //call a function to format messages  
            List<ITweet> tweets = FinalMessage(splettedMessageTweets);

            return Enumerable.DefaultIfEmpty<ITweet>(tweets);
        }

        /// <summary>
        /// Takes array of strings (words) and return a List of Twitter messages without 
        /// applying string formatter. It ensures that a word is not splitted inside words. 
        /// It also checks if words contaions URL and deal with it accodinly.
        /// </summary>
        /// <param name="wordsSpleted"></param>
        /// <param name="wordSplitSize"></param>
        /// <returns></returns>
        List<string> TwitterSplit(string[] wordsSpleted, int wordSplitSize)
        {
            List<string> listOfArrays = new List<string>();
            string sentence = "";
            int numberOfIcons = 9;
            mentionListTostring = MentionGet(wordsSpleted);

            wordsSpleted= wordsSpleted[0].StartsWith("@") ? wordsSpleted.Skip(1).ToArray(): wordsSpleted;

            Regex rgs = new Regex(_UrlRegex);

            foreach (var word in wordsSpleted)
            {
                int wordsLenght = SentactewithUrl(word);
                int centanceLenght = SentactewithUrl(sentence);
           
                if (centanceLenght + wordsLenght + numberOfIcons + mentionListTostring.Length < wordSplitSize)
                {
                    sentence += string.IsNullOrEmpty(sentence) ? word : " " + word;
                }else{
                    listOfArrays.Add(sentence);
                    sentence = word;
                }
            }
            listOfArrays.Add(sentence);
            return listOfArrays;
        }

        /// <summary>
        /// The function apply Config TweetFormat, it returns a list of type ITweet, at first i was utilising Case stement 
        /// and was chnaged to if statement for human readability.
        /// </summary>
        /// <param name="mesages"></param>
        /// <returns></returns>
        List<ITweet> FinalMessage(List<string> mesages)
        {
            List<ITweet> FormatedMessageList = new List<ITweet>();
           
            for (int i = 0; i < mesages.Count; i++)
            {
                var TwitterMessagePrppe = new TwitterMessagePrpperties
                {
                    index = i + 1,
                    total = mesages.Count,
                    mention = mentionListTostring,
                    continuation = _ContinuationText,
                    message = mesages[i],
                    continues = _ContinuesText
                };

                FinalMessageToTwitter Tweet = new FinalMessageToTwitter {};

                if (i == 0 && mesages.Count-1 != 0)
                {
                    TwitterMessagePrppe.continuation = String.Empty;
                    Tweet.Message = _TweetFormat.FormatWith(TwitterMessagePrppe).Replace("  ", " ");
                    FormatedMessageList.Add(Tweet);
                }
                else if (i == 0 && mesages.Count -1 == 0) {
                    TwitterMessagePrppe.continuation = String.Empty;
                    TwitterMessagePrppe.continues = String.Empty;
                    Tweet.Message = _TweetFormat.FormatWith(TwitterMessagePrppe).Replace("  ", " ");
                    FormatedMessageList.Add(Tweet);
                }
                else if (i != 0 && i < mesages.Count - 1)
                {
                    Tweet.Message = _TweetFormat.FormatWith(TwitterMessagePrppe).Replace("  ", " ");
                    FormatedMessageList.Add(Tweet);
                }
                else
                {
                    TwitterMessagePrppe.continues = String.Empty;
                    Tweet.Message = _TweetFormat.FormatWith(TwitterMessagePrppe).Replace("  ", " ");
                    FormatedMessageList.Add(Tweet);
                }
            }

            return FormatedMessageList;
        }

        /// <summary>
        /// Concatinate the string for Mintions 
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>

        string MentionGet(string [] words) {
            string[] listOfMentions =Array.FindAll(words, s => s.StartsWith("@"));
            string mentinListToString = listOfMentions.Length > 0 ? string.Join(" ",listOfMentions) : "";

            return mentinListToString;

        }
        /// <summary>
        /// This function does one thing only to return int length without URL in it.
        /// </summary>
        /// <param name="sentanceWord"></param>
        /// <returns></returns>
        int SentactewithUrl( string sentanceWord) 
        {
            Regex regex = new Regex(_UrlRegex);
            int centanceLenght = 0;

            if (regex.IsMatch(sentanceWord))
            {
                string result = regex.Replace(sentanceWord, "").Replace("  ", " ");
                centanceLenght = result.Length + _ShortUrlCharLength;
            }
            else
            {
                centanceLenght = sentanceWord.Length;
            }
            return centanceLenght;
        }
    }
/// <summary>
/// The class was created to host variables to format Twitter StringFomat
/// </summary>
    public class TwitterMessagePrpperties 
    {
        public int index { get; set; }
        public int total { get; set; }
        public string mention { get; set; }
        public string continuation { get; set; }
        public string message { get; set; }
        public string continues { get; set; }
    }
/// <summary>
/// ITweet Interface must be implemented, this class implements ITweet.
/// </summary>
    public class FinalMessageToTwitter : ITweet
    {
        public string Message { get ; set ; }
    }
}
