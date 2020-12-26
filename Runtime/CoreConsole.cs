namespace com.faith.coreconsole
{
    using UnityEngine;

    public static class CoreConsole
    {
        #region Global Access Point

        public const string _debugMessagePrefix = "[CoreDebug]";

        #endregion

        #region Custom Variables

        [System.Serializable]
        public class DebugInfo
        {
            public string timeStamp;
            public string condition;
            public string stackTrace;
            public UnityEngine.LogType logType;
        }

        #endregion

        #region Section   :   LogWarning  :   Verbose

        public static void LogWarning(object message, Color color = new Color(), string prefix = "")
        {
            if (CoreConsoleConfiguretionFile.GlobalLogType == CoreConsoleEnums.LogType.Verbose)
            {
                string hexColorPrefix = "";
                string hexColorPostfix = "";
                if (color != new Color())
                {
                    color.a = 1;
                    hexColorPrefix = string.Format("<color={0}>", CoreConsoleUtility.GetHexColorFromRGBColor(color));
                    hexColorPostfix = "</color>";
                }


                UnityEngine.Debug.LogWarning(string.Format("{0}{1}{2}{3}{4}",
                    hexColorPrefix,
                    _debugMessagePrefix,
                    prefix == "" ? ": " : "_[" + prefix + "]: ",
                    message,
                    hexColorPostfix));
            }
        }

        public static void LogWarning(object message, Object context, Color color = new Color(), string prefix = "")
        {
            if (CoreConsoleConfiguretionFile.GlobalLogType == CoreConsoleEnums.LogType.Verbose)
            {

                string hexColorPrefix = "";
                string hexColorPostfix = "";
                if (color != new Color())
                {
                    color.a = 1;
                    hexColorPrefix = string.Format("<color={0}>", CoreConsoleUtility.GetHexColorFromRGBColor(color));
                    hexColorPostfix = "</color>";
                }

                UnityEngine.Debug.LogWarning(string.Format("{0}{1}{2}{3}{4}",
                    hexColorPrefix,
                    _debugMessagePrefix,
                    prefix == "" ? ": " : "_[" + prefix + "]: ",
                    message,
                    hexColorPostfix),
                    context);
            }

        }

        #endregion

        //-----------------
        #region Section   :   Log     :   Verbose|Info

        public static void Log(object message, Color color = new Color(), string prefix = "")
        {

            if (CoreConsoleConfiguretionFile.GlobalLogType == CoreConsoleEnums.LogType.Verbose || CoreConsoleConfiguretionFile.GlobalLogType == CoreConsoleEnums.LogType.Info)
            {

                string hexColorPrefix = "";
                string hexColorPostfix = "";
                if (color != new Color())
                {
                    color.a = 1;
                    hexColorPrefix = string.Format("<color={0}>", CoreConsoleUtility.GetHexColorFromRGBColor(color));
                    hexColorPostfix = "</color>";
                }

                UnityEngine.Debug.Log(string.Format("{0}{1}{2}{3}{4}",
                    hexColorPrefix,
                    _debugMessagePrefix,
                    prefix == "" ? ": " : "_[" + prefix + "]: ",
                    message,
                    hexColorPostfix));
            }

        }

        public static void Log(object message, Object context, Color color = new Color(), string prefix = "")
        {

            if (CoreConsoleConfiguretionFile.GlobalLogType == CoreConsoleEnums.LogType.Verbose || CoreConsoleConfiguretionFile.GlobalLogType == CoreConsoleEnums.LogType.Info)
            {

                string hexColorPrefix = "";
                string hexColorPostfix = "";
                if (color != new Color())
                {
                    color.a = 1;
                    hexColorPrefix = string.Format("<color={0}>", CoreConsoleUtility.GetHexColorFromRGBColor(color));
                    hexColorPostfix = "</color>";
                }

                UnityEngine.Debug.Log(string.Format("{0}{1}{2}{3}{4}",
                    hexColorPrefix,
                    _debugMessagePrefix,
                    prefix == "" ? ": " : "_[" + prefix + "]: ",
                    message,
                    hexColorPostfix),
                    context);
            }

        }

        #endregion

        //-----------------
        #region Section   :   LogError    :   Verbose|Info|Error

        public static void LogError(object message, Color color = new Color(), string prefix = "")
        {

            if (CoreConsoleConfiguretionFile.GlobalLogType == CoreConsoleEnums.LogType.Verbose || CoreConsoleConfiguretionFile.GlobalLogType == CoreConsoleEnums.LogType.Info || CoreConsoleConfiguretionFile.GlobalLogType == CoreConsoleEnums.LogType.Error)
            {

                string hexColorPrefix = "";
                string hexColorPostfix = "";
                if (color != new Color())
                {
                    color.a = 1;
                    hexColorPrefix = string.Format("<color={0}>", CoreConsoleUtility.GetHexColorFromRGBColor(color));
                    hexColorPostfix = "</color>";
                }

                UnityEngine.Debug.LogError(string.Format("{0}{1}{2}{3}{4}",
                    hexColorPrefix,
                    _debugMessagePrefix,
                    prefix == "" ? ": " : "_[" + prefix + "]: ",
                    message,
                    hexColorPostfix));
            }

        }

        public static void LogError(object message, Object context, Color color = new Color(), string prefix = "")
        {

            if (CoreConsoleConfiguretionFile.GlobalLogType == CoreConsoleEnums.LogType.Verbose || CoreConsoleConfiguretionFile.GlobalLogType == CoreConsoleEnums.LogType.Info || CoreConsoleConfiguretionFile.GlobalLogType == CoreConsoleEnums.LogType.Error)
            {

                string hexColorPrefix = "";
                string hexColorPostfix = "";
                if (color != new Color())
                {
                    color.a = 1;
                    hexColorPrefix = string.Format("<color={0}>", CoreConsoleUtility.GetHexColorFromRGBColor(color));
                    hexColorPostfix = "</color>";
                }

                UnityEngine.Debug.LogError(string.Format("{0}{1}{2}{3}{4}",
                    hexColorPrefix,
                    _debugMessagePrefix,
                    prefix == "" ? ": " : "_[" + prefix + "]: ",
                    message,
                    hexColorPostfix),
                    context);
            }

        }

        #endregion
    }
}

