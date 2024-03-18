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
        public void PrintTweetsForMultipleUsersAndTweets_Should_OrganiseDataCorrectly()
        {
            // Arrange
            userFilePath = INPUT_DATA_ROOT + "/user1.txt";
            tweetFilePath = INPUT_DATA_ROOT + "/tweet1.txt";

            // Act
            var users = _service.ReadUserFile(userFilePath);
            var tweets = _service.ReadTweetFile(tweetFilePath);

            // Assert
            Assert.That(users.Count, Is.EqualTo(7));
            Assert.That(tweets.Count, Is.EqualTo(10));
        }

        [Test]
        public void PrintTweetsForMultipleUsersAndTweets_Should_ProduceTheCorrectOutput()
        {
            // Arrange
            userFilePath = INPUT_DATA_ROOT + "/user1.txt";
            tweetFilePath = INPUT_DATA_ROOT + "/tweet1.txt";

            var userFollowers = _service.ReadUserFile(userFilePath);
            var tweets = _service.ReadTweetFile(tweetFilePath);


            var sortedUsers = userFollowers.Keys.OrderBy(u => u).ToList();
            using (StringWriter sw = new StringWriter())
            {
                System.Console.SetOut(sw);

                // Act
                sortedUsers.ForEach((user) =>
                {
                    _service.PrintTweetsForUserAndFollowers(user, tweets, userFollowers);
                });

                // Assert
                var expectedResponse = "Alan\n\t@Alan: If you have a procedure with 10 parameters, you probably missed some.\n\t@Jack: Real programmers can write assembly code in any language.\n\t@Martin: Any code of your own that you haven't looked at for six or more months might as well have been written by someone else.\n\t@Alan: Random numbers should not be generated with a method chosen at random.\n\t@Martin: There's no place like 127.0.0.1.\nEmma\n\t@Martin: Any code of your own that you haven't looked at for six or more months might as well have been written by someone else.\n\t@Sarah: Programming is like sex. One mistake and you have to support it for the rest of your life.\n\t@Lily: It's not a bug — it's an undocumented feature.\n\t@Emma: Debugging is like hunting elephants. You only get a feel for how big they are when you've taken one down.\n\t@Martin: There's no place like 127.0.0.1.\nJack\n\t@Jack: Real programmers can write assembly code in any language.\n\t@Martin: Any code of your own that you haven't looked at for six or more months might as well have been written by someone else.\n\t@Martin: There's no place like 127.0.0.1.\nLily\n\t@Alan: If you have a procedure with 10 parameters, you probably missed some.\n\t@Alan: Random numbers should not be generated with a method chosen at random.\n\t@Lily: It's not a bug — it's an undocumented feature.\nMartin\n\t@Martin: Any code of your own that you haven't looked at for six or more months might as well have been written by someone else.\n\t@Emma: Debugging is like hunting elephants. You only get a feel for how big they are when you've taken one down.\n\t@Martin: There's no place like 127.0.0.1.\nSarah\n\t@Sarah: Programming is like sex. One mistake and you have to support it for the rest of your life.\n\t@Lily: It's not a bug — it's an undocumented feature.\nWard\n\t@Ward: There are only two hard things in Computer Science: cache invalidation, naming things and off-by-1 errors.\n\t@Sarah: Programming is like sex. One mistake and you have to support it for the rest of your life.\n\t@Emma: Debugging is like hunting elephants. You only get a feel for how big they are when you've taken one down.\n\t@Ward: To iterate is human, to recurse divine.\n";
                var result = sw.ToString();
                Assert.That(expectedResponse, Is.EqualTo(sw.ToString()));
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
