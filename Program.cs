using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwitterFeedSimulator.Domain.Models;
using TwitterFeedSimulator.Application.Services;
using TwitterFeedSimulator.Application.Interfaces;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TwitterFeedSimulator.Tests")]
namespace TwitterFeedSimulator
{
    internal class Program
    {
        public static void Main(string[] args)
        {

            // Instantiate instance of TwitterFeedSimulatorClass
            var _twitterFeedSimulator = new TwitterFeedSimulatorService();

            const string INPUT_DATA_ROOT = @"../../../Domain/Core/Stubs";

            string userFilePath = INPUT_DATA_ROOT + "/user.txt";
            string tweetFilePath = INPUT_DATA_ROOT + "/tweet.txt";

            // Read users and followers
            var userFollowers = _twitterFeedSimulator.ReadUserFile(userFilePath);

            // Read tweets
            var tweets = _twitterFeedSimulator.ReadTweetFile(tweetFilePath);

            // Sort users alphabetically
            var sortedUsers = userFollowers.Keys.OrderBy(u => u).ToList();

            // Print tweets for each user and their followers
            sortedUsers.ForEach((user) =>
            {
                _twitterFeedSimulator.PrintTweetsForUserAndFollowers(user, tweets, userFollowers);
            });
        }
    }
}
