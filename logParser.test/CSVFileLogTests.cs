using System;
using Xunit;

namespace logParser.test
{
    public class CSVFileTests
    {
        [Fact]
        public void TestAddLastIndexToCsv()
        {
            CSVFileLog.AddLastLogIndexToCSV(3);
            CSVFileLog instance=new CSVFileLog(' ',Levels.INFO,DateTime.Now,"something");
            Assert.Equal(3,instance.LogID);
        }
        
    }
}
