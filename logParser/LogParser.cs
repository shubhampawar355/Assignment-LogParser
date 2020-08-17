using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace logParser {

    class LogParser {
        public static int BlankLines = 0;
        public static int NotInFormatLines = 0;

        static void Main (string[] args) {
            //program needs atleast two args(source .log file folder and destination .csv file ) else it will print help
            if (args.Length < 2) {
                LogParser.PrintHelp ();
                return;
            }
            try {
                //To evaluate all input accordingly
                UserInput userInput = UserInput.GetInstance (args);
                if (!Directory.Exists (userInput.Source)) {
                    System.Console.WriteLine ("Source directory = " + userInput.Source + " is Invalid!! Please enter valid source directory.");
                } else {
                    //creating directories for out path if not present && attach out-file name if not given
                    userInput.ProcessDestinationPath ();
                    //getting all files with .log extenssion from log files directory and its all sub-directories recursievly
                    string[] Files = Directory.GetFiles (userInput.Source, "*.log", SearchOption.AllDirectories);
                    //If there is no .log files in folder 
                    if (Files.Length == 0) {
                        System.Console.WriteLine ("Source directory does not contain any *.log file ");
                        return;
                    }
                    // It will fetch last log record index and add 1 to it if csv out-file already exists otherwise continue
                    CSVFileLog.AddLastLogIndexToCSV (userInput.LastLogIndex);

                    foreach (var file in Files) {
                        //to store all Processed logs which will stored in file after processing all logs from current file 
                        //to save time requiered to open and close connection for each line 
                        List<CSVFileLog> logsToWriteInFile = new List<CSVFileLog> ();

                        List<string> linesFromThisFile = File.ReadAllLines (file).ToList ();

                        foreach (string line in linesFromThisFile) {
                            //Extracting content from .log file from each line storing it in its Equivalent POCO
                            LogFileLog unprocessedLog = new LogFileLog (line, ' ');
                            try {
                                //restricts logs which user don't want in .csv file on basis of Level
                                if (userInput.UserGivenLevels.Contains (unprocessedLog.Level)) {
                                    //to convert log in proper format and getting log ready to write
                                    CSVFileLog processedLog = unprocessedLog.ConvertToCSVFileLog ();
                                    //to give ease if wants to change delimeter 
                                    processedLog.Delimeter = '|';
                                    //adding in list to write
                                    logsToWriteInFile.Add (processedLog);
                                }
                            }
                            //if log format is not matched it will skip that log line 
                            catch (NullReferenceException) { }
                        }
                        //Appending lines to .csv file for each .log file
                        CSVFileLog.WriteToFile (userInput.Destination, '|', logsToWriteInFile);
                    }
                }
                int totoal = BlankLines + NotInFormatLines;
                System.Console.WriteLine ("Log Parsing done Successfully. Skipped lines:" + totoal + "( Blank:" + BlankLines + ", Not_In_Format:" + NotInFormatLines + " )\nOutput file path :" + userInput.Destination);

            }
            //if level given by user is wrong
            catch (ArgumentException) {
                System.Console.WriteLine ("Invalid Level !!! Use any of <INFO|WARN|DEBUG|TRACE|ERROR|EVENT> ");
            }
            //Avoid abnormal termination if any other exception occures at runtime
            catch (Exception excp) {
                System.Console.WriteLine (excp.ToString ());
            }
            //good practice 
            finally { }
        }

        static void PrintHelp () {
            System.Console.WriteLine ("\nUse case 1) logParser --log-dir <dir> --log-level <level> --csv <out> \n" +
                "--log-dir   ==> <Directory to parse recursively for .log files >\n" +
                "    --csv   ==> <Out file-path (absolute/relative)>\n" +
                "--log-level ==> <INFO|WARN|DEBUG|TRACE|ERROR|EVENT> default will be all levels or you can give more than one by giving space between  \n" +
                "\nUse case 2) logParser <Source dir> <level> <Destination dir>\n" +
                "\nUse caese 3) logParser <Source dir> <Destination dir>\n" +
                "               In this case all levels will be considered\n");
        }

    }
}