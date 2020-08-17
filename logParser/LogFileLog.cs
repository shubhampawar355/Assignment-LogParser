using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace logParser {

   public class LogFileLog {
        private static int count = 1;
        public int LogID { get; set; }
        public char Delimeter { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public Levels Level { get; set; }
        public string Info { get; set; }

        public static void SetCount (int LastLogId) {
            count = LastLogId;
        }

        public LogFileLog (char delim, Levels level, string date, string time, string info) {
            this.Delimeter = delim;
            this.LogID = count++;
            this.Level = level;
            this.Date = date;
            this.Time = time;
        }
        public LogFileLog (string line, char delim) {
            this.Delimeter = delim;
            var TabSpace = new String (' ', 4);
            line = line.Replace ("\t", TabSpace);
            string[] splitedLineArray = line.Split (Delimeter, StringSplitOptions.RemoveEmptyEntries);
            if (splitedLineArray.Length > 3) {
                try {
                    this.Level = (Levels) Enum.Parse (typeof (Levels), splitedLineArray[2], true);
                    this.Date = splitedLineArray[0];
                    this.Time = splitedLineArray[1];
                    this.Info = string.Join (" ", splitedLineArray.Skip (3));
                    this.LogID = count++;
                } catch (Exception) {++LogParser.NotInFormatLines; }
            } else {
                if (splitedLineArray.Length <= 0)
                    ++LogParser.BlankLines;
                else
                    ++LogParser.NotInFormatLines;
            }
        }

        public CSVFileLog ConvertToCSVFileLog () {
            string[] SplitDate = Date.Split ('/');
            DateTime date = new DateTime (DateTime.Now.Year, Int32.Parse (SplitDate[0]), Int32.Parse (SplitDate[1]));
            string[] SplitTime = Time.Split (':');
            TimeSpan ts = new TimeSpan (Int16.Parse (SplitTime[0]), Int16.Parse (SplitTime[1]), Int16.Parse (SplitTime[2]));
            DateTime ActualDate = date.Add (ts);
            return new CSVFileLog (Delimeter, Level, ActualDate, Info);
        }

        public override string ToString () {
            string str = Date + Delimeter + Time + Delimeter + Level.ToString () + Delimeter + Info;
            return str;
        }

        public void WriteToFile (string destination) {
            File.AppendAllText (destination, this.ToString ());
        }

        public static void WriteToFile (string destination, List<LogFileLog> list) {
            List<string> ls = new List<string> ();
            foreach (var item in list) {
                ls.Add (item.ToString ());
            }
            File.AppendAllLines (destination, ls);
        }
    }
}