using System;
using TwitterFeedSimulator.Application.Interfaces;
using TwitterFeedSimulator.Domain.Models;

namespace TwitterFeedSimulator.Application.Services
{
	public class TwitterFeedSimulatorService : ITwitterFeedSimulatorService
	{
		public TwitterFeedSimulatorService()
		{
		}

        /// <summary>
        /// This method will produce the final output
        /// Meaning it will display the list of users and the associated tweets they are able to view
        /// </summary>
        /// <param name="user"> STRING: Username of the user </param>
        /// <param name="tweets"> List<Tweet>: List of tweets of the associated user </param>
        /// <param name="userFollowers"> Dictionary<string, List<string>>: Dictionary containing <user>:<users_that_they_follow> </param>
        public void PrintTweetsForUserAndFollowers(string user, List<Tweet> tweets, Dictionary<string, List<string>> userFollowers)
        {
			// Print the users name
			Console.WriteLine(user);

            // Using a HashSet ensures that each user and follower is unique and eliminates any duplicates that might arise from the user file.
			// This is important because a user can follow multiple other users, and we want to ensure that each user's tweets are printed only once,
			// even if they are followed by multiple users
            var userAndFollowers = new HashSet<string>(userFollowers[user]);

			// We need to include the user's own tweets
            userAndFollowers.Add(user);

			// Filter the list of tweets that the user should be able to see
            var userTweets = tweets.Where(tweet => userAndFollowers.Contains(tweet.User)).ToList();

			// Print the tweet and the person who tweeted it
			userTweets.ForEach((tweet) =>
			{
				Console.WriteLine($"\t@{tweet.User}: {tweet.Message}");
			});
        }

        /// <summary>
        /// This method reads the input data from the provided textfile
		/// This text file contains the list of tweets
        /// </summary>
        /// <param name="tweetFilePath">STRING: File path of the text file containing the tweet data</param>
        /// <returns>List of tweets</returns>
        public List<Tweet> ReadTweetFile(string tweetFilePath)
        {
			// Initialise empty response
			var result = new List<Tweet>();

			// Read all lines from the text file
			List<string> lines = File.ReadAllLines(tweetFilePath).ToList();

			// Go through each line
			lines.ForEach((line) =>
			{
				// Split to get the user and the associated tweet
				var partsOfLine = line.Split("> ").ToList();

				var user = partsOfLine.FirstOrDefault();
				var message = partsOfLine.LastOrDefault();

				// Add to Tweet model
				result.Add(new Tweet(user, message));
			});

			return result;
        }

        /// <summary>
        /// This method reads the input data from the provided textfile
		/// This text file contains the list of users and their followers
        /// </summary>
        /// <param name="userFilePath">STRING: File path of the text file containing the tweet data</param>
        /// <returns>Dictionary of users and a list of their followers</returns>
        public Dictionary<string, List<string>> ReadUserFile(string userFilePath)
        {
			// Initialise 
			var result = new Dictionary<string, List<string>>();

            // Read all lines from the text file
            List<string> lines = File.ReadAllLines(userFilePath).ToList();

            // Go through each line
            lines.ForEach((line) =>
			{
				// Split to get the user and their associated followers
				var partsOfLine = line.Split(" follows ");

				var user = partsOfLine.FirstOrDefault();
				var following = partsOfLine.LastOrDefault();

				// Create a list structure for the associated users' followers
				result[user] = RetrieveUserFollowers(following);

				// find any missing entries -> "follower users" are still users to be considered
				var followingList = following.Split(", ").ToList();

				// Add these users to the list
				followingList.ForEach((userFollowing) =>
				{
					if (!result.ContainsKey(userFollowing))
					{
						result[userFollowing] = new List<string>();
					}
				});


			});
			return result;
        }

		private List<string> RetrieveUserFollowers(string followers)
		{
			return followers.Split(", ").ToList();
		}
    }
}

