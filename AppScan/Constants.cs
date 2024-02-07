using System;
using System.IO;

namespace Common {
    public static class Constants {
        public static readonly string PIPE_NAME = "Proximity.Library";

        private const string LOG_FOLDER = "Logs";
        private const string SOTI_FOLDER = "SOTI";
        private const string PROXIMITY_FOLDER = "Proximity";
        private const string PROXIMITY_UI_LOG = "Proximity_UI_Log_.txt";
        private const string PROXIMITY_SERVICE_LOG = "Proximity_Service_Log_.txt";

        private static readonly string PROGRAM_DATA_PATH =
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        private static readonly string OBD_PATH = Path.Combine(SOTI_FOLDER, PROXIMITY_FOLDER);
        private static readonly string BASE_DIRECTORY_PATH = Path.Combine(PROGRAM_DATA_PATH, OBD_PATH);
        private static readonly string LOG_PATH = Path.Combine(BASE_DIRECTORY_PATH, LOG_FOLDER);

        public static readonly string UI_LOG_PATH = Path.Combine(LOG_PATH, PROXIMITY_UI_LOG);
        public static readonly string SERVICE_LOG_PATH = Path.Combine(LOG_PATH, PROXIMITY_SERVICE_LOG);
    }
}