using System;
using System.Linq;

namespace Core
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Please provide exactly 1 argument - the text to split.");
                return;
            }

            var messageInput = args[0];

            var configuration = new SplitterConfiguration();
            var splitter = new Splitter(configuration); ;
            
            Console.WriteLine("Splitting message into Tweets.");
            var tweets = splitter.Split(messageInput);
            
            Console.WriteLine($"Tweets created: {tweets.Count()}.");

            for (var i = 0; i < tweets.Count(); i++)
            {
                var tweet = tweets.ElementAt(i);
                Console.WriteLine($"Printing Tweet {i} of {tweets.Count()} ({tweet.Message.Length}) characters.");
                Console.WriteLine(tweet.Message);
                Console.WriteLine();
            }

            Console.WriteLine($"Press any key to continue.");
            Console.ReadKey();
        }
    }
}
