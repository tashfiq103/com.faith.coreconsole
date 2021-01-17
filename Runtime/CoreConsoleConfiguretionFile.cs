namespace com.faith.coreconsole
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;


    public class CoreConsoleConfiguretionFile : ScriptableObject
    {
        #region Global

        public static CoreConsoleEnums.LogType GlobalLogType = CoreConsoleEnums.LogType.Verbose;

        #endregion

        #region SerializedField

        [SerializeField] private bool _isMarkedAsDefaultSetting = false;
        [SerializeField] private bool _isLinkedWithDefaultSetting = false;

        [SerializeField] private string _name;
        [SerializeField] private bool _enableStackTrace;
        [SerializeField, Range(10, 999)] private int _numberOfLog = 100;
        [SerializeField] private LogType _clearLogType;
        [SerializeField] private List<CoreConsole.DebugInfo> _listOfLogInfo = new List<CoreConsole.DebugInfo>();

        [SerializeField] private CoreConsoleEnums.LogType _logType = CoreConsoleEnums.LogType.Verbose;

        #endregion

        #region Public Variables

#if UNITY_EDITOR

        
        public List<CoreConsole.DebugInfo> EditorListOfLogInfo { get { return _listOfLogInfo; } }

#endif

        public int NumberOfLog { get { return _listOfLogInfo.Count; } }
        public CoreConsoleEnums.LogType logType { get { return _isLinkedWithDefaultSetting ? GlobalLogType : _logType; } }

        public string prefix = "";
        public Color colorForLog = new Color();
        public Color colorForWarning = Color.yellow;
        public Color colorForLogError = Color.red;

        #endregion

        //--------------------
        //Region Seperator
        //--------------------
        #region ScriptableObject

#if UNITY_EDITOR


#endif
        private void OnEnable()
        {
            
            if (_enableStackTrace)
            {
                if (_listOfLogInfo == null) _listOfLogInfo = new List<CoreConsole.DebugInfo>();

                Application.logMessageReceivedThreaded += LogMessageReciever;
            }
        }

        private void OnDisable()
        {
            if (_enableStackTrace)
            {
                Application.logMessageReceivedThreaded -= LogMessageReciever;
            }
        }


        #endregion

        //--------------------
        //Region Seperator
        //--------------------
        #region Configuretion

        private void LogMessageReciever(string condition, string stackTrace, UnityEngine.LogType logType)
        {

            if (string.IsNullOrEmpty(prefix) || string.IsNullOrWhiteSpace(prefix))
            {
                Debug.LogWarning(string.Format("No prefix was found for [{0}]_['{1}']. Assigning it's name as new prefix = {2}", CoreConsole.DEBUG_MESSAGE_PREFIX, prefix, name));
                prefix = name;
                return;
            }

            string filter = string.Format("{0}_[{1}]", CoreConsole.DEBUG_MESSAGE_PREFIX, prefix);
            if (condition.Contains(filter))
            {

                if (_listOfLogInfo.Count >= _numberOfLog)
                    _listOfLogInfo.RemoveAt(0);

                _listOfLogInfo.Add(new CoreConsole.DebugInfo()
                {
                    timeStamp = DateTime.Now.ToString(),
                    prefix = prefix,
                    condition = condition,
                    stackTrace = stackTrace,
                    logType = logType
                });
            }
        }

        #endregion

        //--------------------
        //Region Seperator
        //--------------------
        #region Public Callback

        public void ClearAllLog()
        {
            _listOfLogInfo.Clear();
        }

        public void ClearLog(UnityEngine.LogType logType)
        {

            List<int> listOfRemovingIndex = new List<int>();
            int numberOfLog = _listOfLogInfo.Count;

            for (int i = 0; i < numberOfLog; i++)
            {
                if (_listOfLogInfo[i].logType == logType)
                    listOfRemovingIndex.Add(i);
            }

            int leftShiftValue = 0;
            foreach (int index in listOfRemovingIndex)
            {
                _listOfLogInfo.RemoveAt(index - leftShiftValue);
                leftShiftValue++;
            }
        }

        public int GetNumberOfLog(UnityEngine.LogType logType)
        {

            int numberOfLog = 0;
            foreach (CoreConsole.DebugInfo debugInfo in _listOfLogInfo)
            {

                if (logType == debugInfo.logType)
                    numberOfLog++;
            }

            return numberOfLog;
        }

        #endregion
    }

}



