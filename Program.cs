using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TwitterFeedSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: TwitterFeedSimulator <user_file_path> <tweet_file_path>");
                // return;
            }

            // string userFilePath = args[0];
            // string tweetFilePath = args[1];
            string userFilePath = "/Users/seyurlutchminarain/Desktop/TwitterFeedSimulator/user.txt";
            string tweetFilePath = "/Users/seyurlutchminarain/Desktop/TwitterFeedSimulator/tweet.txt";

            // Read users and followers
            Dictionary<string, List<string>> userFollowers = ReadUserFile(userFilePath);

            // Read tweets
            List<Tweet> tweets = ReadTweetFile(tweetFilePath);

            // Sort users alphabetically
            var sortedUsers = userFollowers.Keys.OrderBy(u => u).ToList();

            // Print tweets for each user and their followers
            foreach (var user in sortedUsers)
            {
                Console.WriteLine(user);
                PrintTweetsForUserAndFollowers(user, tweets, userFollowers);
            }
        }

        static Dictionary<string, List<string>> ReadUserFile(string userFilePath)
        {
            var userFollowers = new Dictionary<string, List<string>>();
            foreach (var line in File.ReadLines(userFilePath))
            {
                var parts = line.Split(" follows ");
                var user = parts[0];
                var followers = parts[1].Split(", ");
                userFollowers[user] = followers.ToList();

                foreach (var follower in followers)
                {
                    if (!userFollowers.ContainsKey(follower))
                    {
                        userFollowers[follower] = new List<string>();
                    }
                }
            }
            return userFollowers;
        }

        static List<Tweet> ReadTweetFile(string tweetFilePath)
        {
            var tweets = new List<Tweet>();
            foreach (var line in File.ReadLines(tweetFilePath))
            {
                var parts = line.Split("> ");
                var user = parts[0];
                var message = parts[1];
                tweets.Add(new Tweet(user, message));
            }
            return tweets;
        }

        static void PrintTweetsForUserAndFollowers(string user, List<Tweet> tweets, Dictionary<string, List<string>> userFollowers)
        {
            var userAndFollowers = new HashSet<string>(userFollowers[user]);
            userAndFollowers.Add(user); // Include user's own tweets
            var userTweets = tweets.Where(tweet => userAndFollowers.Contains(tweet.User));
            foreach (var tweet in userTweets)
            {
                Console.WriteLine($"\t@{tweet.User}: {tweet.Message}");
            }
        }
    }

    class Tweet
    {
        public string User { get; }
        public string Message { get; }

        public Tweet(string user, string message)
        {
            User = user;
            Message = message;
        }
    }
}
