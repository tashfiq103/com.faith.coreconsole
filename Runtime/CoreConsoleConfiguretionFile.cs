namespace com.faith.coreconsole
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

#if UNITY_EDITOR
    using System.Threading.Tasks;
#endif

    [CreateAssetMenu]
    public class CoreConsoleConfiguretionFile : ScriptableObject
    {
        #region Global

        public static CoreConsoleEnums.LogType GlobalLogType = CoreConsoleEnums.LogType.Verbose;

        #endregion

        #region SerializedField

        [SerializeField] private bool _isUsedByCentralCoreConsole = false;
        [SerializeField] private bool _linkWithCentralCoreConsole = false;

        [SerializeField] private bool _enableStackTrace;
        [SerializeField, Range(10, 999)] private int _numberOfLog = 100;
        [SerializeField] private LogType _clearLogType;
        [SerializeField] private List<CoreConsole.DebugInfo> _listOfLogInfo = new List<CoreConsole.DebugInfo>();

        [SerializeField] private CoreConsoleEnums.LogType _logType = CoreConsoleEnums.LogType.Verbose;

        #endregion

        #region Public Variables

#if UNITY_EDITOR

        public bool EditorAccessIfUsedByCentralCoreConsole
        {
            get
            {
                return _isUsedByCentralCoreConsole;
            }
        }
        public bool EditorAccessIfLinkWithCentralCoreConsole
        {
            get
            {
                return _linkWithCentralCoreConsole;
            }
        }

        public List<CoreConsole.DebugInfo> EditorListOfLogInfo { get { return _listOfLogInfo; } }

#endif

        public int NumberOfLog { get { return _listOfLogInfo.Count; } }
        public CoreConsoleEnums.LogType logType { get { return _linkWithCentralCoreConsole ? GlobalLogType : _logType; } }

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

        private void Awake()
        {
            CheckDuplicateProductionAssetWithDelay();
        }

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

        #region Configuretion

        //--------------------
        //Region Seperator
        //--------------------

#if UNITY_EDITOR

        private async void CheckDuplicateProductionAssetWithDelay()
        {

            await Task.Delay(100);

            if (_isUsedByCentralCoreConsole)
            {

                List<CoreConsoleConfiguretionFile> listOfAsset = new List<CoreConsoleConfiguretionFile>();
                string[] GUIDs = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(CoreConsoleConfiguretionFile).ToString().Replace("UnityEngine.", ""));

                foreach (string GUID in GUIDs)
                {
                    string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(GUID);
                    listOfAsset.Add((CoreConsoleConfiguretionFile)Convert.ChangeType(UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(CoreConsoleConfiguretionFile)), typeof(CoreConsoleConfiguretionFile)));
                }

                foreach (CoreConsoleConfiguretionFile gameConfigAsset in listOfAsset)
                {
                    if (gameConfigAsset != this && gameConfigAsset.EditorAccessIfUsedByCentralCoreConsole)
                    {
                        _isUsedByCentralCoreConsole = false;
                        break;
                    }
                }
            }
        }

#endif

        private void LogMessageReciever(string condition, string stackTrace, UnityEngine.LogType logType)
        {

            if (string.IsNullOrEmpty(prefix) || string.IsNullOrWhiteSpace(prefix))
            {
                Debug.LogWarning(string.Format("No prefix was found for [CoreDebugger]_['{0}']. Assigning it's name as new prefix = {1}", prefix, name));
                prefix = name;
                return;
            }

            string filter = string.Format("{0}_[{1}]", CoreConsole._debugMessagePrefix, prefix);
            if (condition.Contains(filter))
            {

                if (_listOfLogInfo.Count >= _numberOfLog)
                    _listOfLogInfo.RemoveAt(0);

                _listOfLogInfo.Add(new CoreConsole.DebugInfo()
                {
                    timeStamp = DateTime.Now.ToString(),
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



