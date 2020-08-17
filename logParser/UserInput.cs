using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace logParser {
    public sealed class UserInput {
        public string Source { get; set; }
        public string Destination { get; set; }
        public HashSet<Levels> UserGivenLevels { get; set; }
        public int LastLogIndex;

        private static UserInput Instance = null;
        private static readonly object padlock = new object ();

        private UserInput (string[] args) {
            this.UserGivenLevels = new HashSet<Levels> ();

            for (int i = 0; i < args.Length; i++) {
                if (!(args[i] == null) && args[i].StartsWith ("--")) {
                    if (args[i].Equals ("--log-dir")) {
                        Source = @args[i + 1];
                    } else if (args[i].Equals ("--csv")) {
                        Destination = @args[i + 1];
                    } else if (args[i].Equals ("--log-level")) {
                        try {
                            UserGivenLevels.Add ((Levels) Enum.Parse (typeof (Levels), args[i + 1], true));
                        } catch (ArgumentException) {
                            throw;
                        }
                    }
                    args[i] = null;
                    args[i + 1] = null;
                }
            }

            if (Source == null || Destination == null) {
                if (Source == null && Destination == null) {
                    Source = @args[0];
                    args[0] = null;
                    Destination = @args[args.Length - 1];
                    args[args.Length - 1] = null;
                    //AssignLevels (args);
                } else if (Destination == null) {
                    foreach (var item in args) {
                        if (item != null)
                            Destination = item;
                    }
                } else if (Source == null) {
                    foreach (var item in args) {
                        if (item != null)
                            Source = item;
                    }
                }
            }
            AssignLevels (args);
            ProcessDestinationPath ();
            SetLastLogIndex ();
        }

        private void AssignLevels (string[] args) {
            if (args.Length == 2) {
                string[] all = Enum.GetNames (typeof (Levels));
                foreach (var value in all) {
                    UserGivenLevels.Add ((Levels) Enum.Parse (typeof (Levels), value, true));
                }
            } else {
                for (int i = 1; i < args.Length; i++) {
                    if (args[i] != null) {
                        try {
                            UserGivenLevels.Add ((Levels) Enum.Parse (typeof (Levels), args[i], true));
                        } catch (ArgumentException) {
                            throw;
                        }
                    }
                }
            }
        }

        public static UserInput GetInstance (string[] args) {
            lock (padlock) {
                if (Instance == null)
                    Instance = new UserInput (args);
                return Instance;
            }
        }

        public void ProcessDestinationPath () {
            if (!Path.HasExtension (this.Destination)) {
                char[] dest = this.Destination.ToArray ();
                char slash = '\\';
                if (dest[dest.Length - 1] == slash) {
                    this.Destination += "log.csv";
                } else {
                    this.Destination += ".csv";
                }
            }
            Directory.CreateDirectory (Path.GetDirectoryName (this.Destination));
        }

        private void SetLastLogIndex () {
            LastLogIndex = 1;
            if (File.Exists (this.Destination)) {
                try {
                    String last = File.ReadLines (Destination).Last ();
                    string[] split = last.Split ('|');
                    LastLogIndex = Int32.Parse (split[1]) + 1;
                } catch (Exception) { }
            }
        }
    }
}