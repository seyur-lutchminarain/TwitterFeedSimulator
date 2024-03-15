using NUnit.Framework;
using TwitterFeedSimulator.Application.Services;
using TwitterFeedSimulator.Domain.Models;

namespace TwitterFeedSimulator.Tests
{
    [TestFixture]
    public class TwitterFeedSimulatorServiceTests
    {
        private TwitterFeedSimulatorService _service;
        const string INPUT_DATA_ROOT = @"../../../Stubs";
        const string DYNAMIC_STUB_ROOT = @"../../../Stubs/DynamicStubs";

        string userFilePath = INPUT_DATA_ROOT + "/user.txt";
        string tweetFilePath = INPUT_DATA_ROOT + "/tweet.txt";

        [SetUp]
        public void Setup()
        {
            _service = new TwitterFeedSimulatorService();
        }

        [Test]
        public void PrintTweetsForUserAndFollowers_WhenCalled_PrintsCorrectTweets()
        {
            // Arrange
            string user = "user1";
            var tweets = new List<Tweet>
            {
                new Tweet("user1", "Tweet 1"),
                new Tweet("user2", "Tweet 2"),
                new Tweet("user3", "Tweet 3")
            };
            var userFollowers = new Dictionary<string, List<string>>
            {
                { "user1", new List<string>{ "user2", "user3" } },
                { "user2", new List<string>{ "user1" } },
                { "user3", new List<string>{ "user1", "user2" } }
            };

            // Act
            using (StringWriter sw = new StringWriter())
            {
                System.Console.SetOut(sw);
                _service.PrintTweetsForUserAndFollowers(user, tweets, userFollowers);
                var expectedOutput = "user1\n\t@user1: Tweet 1\n\t@user2: Tweet 2\n\t@user3: Tweet 3\n";
                // Assert
                Assert.AreEqual(expectedOutput, sw.ToString());
            }
        }

        [Test]
        public void ReadUserFile_WhenFilePathIsValid_ReturnsDictionaryOfUsersAndFollowers()
        {
            // Arrange & Act
            var result = _service.ReadUserFile(userFilePath);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Dictionary<string, List<string>>>(result);
        }

        [Test]
        public void ReadTweetFile_WhenFilePathIsValid_ReturnsListOfTweets()
        {
            // Act
            var result = _service.ReadTweetFile(tweetFilePath);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<Tweet>>(result);
        }

        [Test]
        public void ReadTweetFile_WithValidFilePath_ReturnsListOfTweets()
        {
            // Arrange
            File.WriteAllText($"{DYNAMIC_STUB_ROOT}/sample.txt", "user1> Tweet 1\nuser2> Tweet 2");

            // Act
            var tweets = _service.ReadTweetFile($"{DYNAMIC_STUB_ROOT}/sample.txt");

            // Assert
            Assert.IsNotNull(tweets);
            Assert.AreEqual(2, tweets.Count);
            Assert.IsTrue(tweets.Any(tweet => tweet.User == "user1" && tweet.Message == "Tweet 1"));
            Assert.IsTrue(tweets.Any(tweet => tweet.User == "user2" && tweet.Message == "Tweet 2"));

            // Cleanup
            File.Delete($"{DYNAMIC_STUB_ROOT}/sample.txt");
        }

        [Test]
        public void ReadUserFile_WithValidFilePath_ReturnsDictionaryOfUsersAndFollowers()
        {
            // Arrange
            File.WriteAllText($"{DYNAMIC_STUB_ROOT}/sample.txt", "user1 follows user2, user3\nuser2 follows user1");

            // Act
            var userFollowers = _service.ReadUserFile($"{DYNAMIC_STUB_ROOT}/sample.txt");

            // Assert
            Assert.IsNotNull(userFollowers);
            Assert.AreEqual(3, userFollowers.Count);
            Assert.IsTrue(userFollowers.ContainsKey("user1"));
            Assert.IsTrue(userFollowers.ContainsKey("user2"));
            Assert.IsTrue(userFollowers.ContainsKey("user3"));
            Assert.AreEqual(2, userFollowers["user1"].Count);
            Assert.AreEqual(1, userFollowers["user2"].Count);
            Assert.AreEqual(0, userFollowers["user3"].Count);

            // Cleanup
            File.Delete($"{DYNAMIC_STUB_ROOT}/sample.txt");
        }

    }
}
