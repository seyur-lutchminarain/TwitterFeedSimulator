using System;
using TwitterFeedSimulator.Domain.Models;

namespace TwitterFeedSimulator.Application.Interfaces
{
	public interface ITwitterFeedSimulatorService
	{
        Dictionary<string, List<string>> ReadUserFile(string userFilePath);
        List<Tweet> ReadTweetFile(string tweetFilePath);
        void PrintTweetsForUserAndFollowers(string user, List<Tweet> tweets, Dictionary<string, List<string>> userFollowers);
    }
}

