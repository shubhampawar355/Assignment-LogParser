using System;
using System.Collections.Generic;
using Xunit;

namespace logParser.test {
    public class UserInputTests {

        [Fact]
        public void  TestAssignLevelProvideDefaultAllLevelsIfNotProvided () {
            string[] levels = Enum.GetNames (typeof (Levels));
            string[] args = new string[] { @"E:\sample\01_cs_fundamentals\01_log_to_csv\logs", @"E:\sample\log.csv" };
            UserInput userInput = UserInput.GetInstance (args);

            HashSet<Levels> expected = new HashSet<Levels> ();
            foreach (string level in levels) {
                expected.Add ((Levels) Enum.Parse (typeof (Levels), level, true));
            }

            bool temp = expected.SetEquals (userInput.UserGivenLevels);
            Assert.True (temp);
            userInput=null;
        }
        //To clear user Instance as it it is singleto and thread safe it will create conflict 
        [Fact]
        public void TestGetInstacneReturnSameInstance () {
            string[] args = new string[] { @"E:\sample\01_cs_fundamentals\01_log_to_csv\logs", @"E:\sample\log.csv" };
            string[] args1 = null;
            UserInput userInputOne = UserInput.GetInstance (args);
            UserInput userInputSecond = UserInput.GetInstance (args1);
            Assert.Equal (userInputOne, userInputSecond);
        }

        [Fact]
        public void TestProvideInputProperlyExtracted () {
            string[] args = new string[] { @"E:\sample\01_cs_fundamentals\01_log_to_csv\logs", "error", @"E:\sample\log.csv" };
            UserInput userInput = UserInput.GetInstance (args);
            Assert.Equal (@"E:\sample\01_cs_fundamentals\01_log_to_csv\logs", userInput.Source);
            Assert.Equal (@"E:\sample\log.csv", userInput.Destination);
            HashSet<Levels> expected = new HashSet<Levels> ();
            expected.Add (Levels.ERROR);
            Assert.Equal (expected, userInput.UserGivenLevels);
        }

        [Theory]
        [InlineData (@"E:\sample\log.csv")]
        [InlineData (@"E:\sample\log")]
        [InlineData (@"E:\sample\")]
        public void TestProcessDestinationWorkFine (string @destination) {
            string expected = @"E:\sample\log.csv";
            string[] args = new string[] { @"E:\sample\01_cs_fundamentals\01_log_to_csv\logs", "Info", destination };
            UserInput userInput = UserInput.GetInstance (args);
            userInput.ProcessDestinationPath ();
            Assert.Equal (expected, userInput.Destination);
        }
    }
}