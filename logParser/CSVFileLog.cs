using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace logParser {

    public class CSVFileLog {
        private static int count = 1;
        public int LogID { get; private set; }
        public char Delimeter { get; set; }
        public Levels Level { get; set; }
        public string Info { get; set; }
        public DateTime Date { get; set; }

        public CSVFileLog (char delim, Levels level, DateTime date, string info) {
            this.Delimeter = delim;
            this.LogID = count++;
            this.Date = date;
            this.Info = info;
            this.Level = level;
        }

        public LogFileLog ConvertToLogFileLog () {
            string date = this.Date.ToString ("MM") + "/" + this.Date.ToString ("dd");
            string time = this.Date.ToLongTimeString ();
            return new LogFileLog (Delimeter, Level, date, time, Info);
        }

        public override string ToString () {
            string str = "" + Delimeter + LogID + Delimeter + Level.ToString () + Delimeter + Date.ToString ("dd MMM yyy") + Delimeter + Date.ToShortTimeString () + Delimeter + Info + Delimeter;
            return str;
        }

        public void WriteToFile (string destination) {
            CSVFileLog.AddHeaderToDestination (destination, Delimeter);
            File.AppendAllText (destination, this.ToString ());
        }

        public static void AddLastLogIndexToCSV (int lastLogId) {
            count = lastLogId;
        }

        public static void WriteToFile (string destination, char delim, List<CSVFileLog> list) {

            List<string> ls = new List<string> ();
            char Delimeter = delim;
            foreach (var item in list) {
                ls.Add (item.ToString ());
            }
            CSVFileLog.AddHeaderToDestination (destination, delim);
            File.AppendAllLines (destination, ls);
        }

        public static void AddHeaderToDestination (string destination, char delim) {
            string str = "" + delim + " No " + delim + " Level " + delim + " Date " + delim + " Time " + delim + " Text " + delim + "\n";
            if (!File.Exists (destination))
                File.Create (destination).Close ();
            if (new FileInfo (destination).Length == 0)
                File.AppendAllText (destination, str);
        }
    }

}