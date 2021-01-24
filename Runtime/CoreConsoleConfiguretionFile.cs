namespace com.faith.coreconsole
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;


    public class CoreConsoleConfiguretionFile : ScriptableObject
    {
        #region Global

        public static CoreConsoleEnums.LogType GlobalLogType    = CoreConsoleEnums.LogType.Verbose;
        public static bool  GlobalEnableStackTrace              = false;
        public static int   GlobalNumberOfLog                   = 100;
        public static Color GlobalColorForLog                   = Color.white;
        public static Color GlobalColorForLogWarning            = Color.yellow;
        public static Color GlobalColorForLogError              = Color.red;

        #endregion

        #region SerializedField

        [SerializeField] private bool _isStackEventRegistered;

        [SerializeField] private bool _isMarkedAsDefaultSetting = false;
        [SerializeField] private bool _isLinkedWithDefaultSetting = false;

        [SerializeField] private string _name;
        [SerializeField] private string _prefix = "";

        [SerializeField] private CoreConsoleEnums.LogType _logType = CoreConsoleEnums.LogType.Verbose;
        [SerializeField] private bool _isStackTraceEnabled;
        [SerializeField, Range(10, 999)] private int _numberOfLog = 100;
        [SerializeField] private Color _colorForLog = Color.white;
        [SerializeField] private Color _colorForWarning = Color.yellow;
        [SerializeField] private Color _colorForLogError = Color.red;

        [SerializeField] private LogType _clearLogType;
        [SerializeField] private List<CoreConsole.DebugInfo> _listOfLogInfo = new List<CoreConsole.DebugInfo>();

        #endregion

        #region Public Variables

#if UNITY_EDITOR


        public List<CoreConsole.DebugInfo> EditorListOfLogInfo { get { return _listOfLogInfo; } }

#endif

        public bool IsDefaultSettings
        {
            get {
                return _isMarkedAsDefaultSetting;
            }
        }
        
        public string Prefix {
            get {
                return _prefix;
            }
        }
        public CoreConsoleEnums.LogType logType {
            get {
                return _isLinkedWithDefaultSetting ? GlobalLogType : _logType;
            }
        }
        public Color ColorForLog {
            get {
                return _isLinkedWithDefaultSetting ? GlobalColorForLog : _colorForLog;
            }
        }
        public Color ColorForWarning
        {
            get
            {
                return _isLinkedWithDefaultSetting ? GlobalColorForLogWarning : _colorForWarning;
            }
        }
        public Color ColorForLogError
        {
            get
            {
                return _isLinkedWithDefaultSetting ? GlobalColorForLogError : _colorForLogError;
            }
        }

        public bool IsStackTraceEnabled
        {
            get {
                return _isLinkedWithDefaultSetting ? GlobalEnableStackTrace : _isStackTraceEnabled;
            }
        }
        public int NumberOfLog
        {
            get {

                return _isLinkedWithDefaultSetting ? GlobalNumberOfLog : _numberOfLog;
            }
        }
        #endregion

        //--------------------
        //Region Seperator
        //--------------------
        #region ScriptableObject
        

        private void OnEnable()
        {
            EnableStackTrace();
        }

        private void OnDisable()
        {
            DisableStackTrace();
        }


        #endregion

        //--------------------
        //Region Seperator
        //--------------------
        #region Configuretion

        

        private void LogMessageReciever(string condition, string stackTrace, UnityEngine.LogType logType)
        {

            string filter = string.Format("{0}_[{1}]", CoreConsole.DEBUG_MESSAGE_PREFIX, _prefix);
            if (condition.Contains(filter))
            {

                if (_listOfLogInfo.Count >= NumberOfLog)
                    _listOfLogInfo.RemoveAt(0);

                _listOfLogInfo.Add(new CoreConsole.DebugInfo()
                {
                    timeStamp = DateTime.Now.ToString(),
                    prefix = _prefix,
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

        public void AssignGlobalValueForDefaultSetting()
        {

            if (_isMarkedAsDefaultSetting)
            {

                GlobalLogType = _logType;

                GlobalEnableStackTrace = _isStackTraceEnabled;
                GlobalNumberOfLog = _numberOfLog;

                GlobalColorForLog = _colorForLog;
                GlobalColorForLogWarning = _colorForWarning;
                GlobalColorForLogError = _colorForLogError;
            }
        }

        public void EnableStackTrace() {

            if (IsStackTraceEnabled && !_isStackEventRegistered) {

                if (_listOfLogInfo == null) _listOfLogInfo = new List<CoreConsole.DebugInfo>();

                Application.logMessageReceivedThreaded += LogMessageReciever;
                _isStackEventRegistered = true;
            }

            
        }

        public void DisableStackTrace()
        {
            if (IsStackTraceEnabled && _isStackEventRegistered)
            {
                Application.logMessageReceivedThreaded -= LogMessageReciever;
                _isStackEventRegistered = false;
            }
        }

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



