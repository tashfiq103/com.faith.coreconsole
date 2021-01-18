namespace com.faith.coreconsole
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;
    using System.Collections.Generic;

    internal class CoreConsoleEditorWindow : CoreConsoleBaseEditorWindowClass, IPreprocessBuildWithReport
    {
        #region Custom Variables

        private class ConsoleDebugInfo
        {
            internal CoreConsoleConfiguretionFile gameConfig;
            internal CoreConsole.DebugInfo debugInfo;
        }

        #endregion

        #region Public Variables

        public int callbackOrder => CoreConsoleConstants.EXECUTION_ORDER_FOR_CONFIGUREION_FILE_CONTAINER;

        #endregion

        #region Private Variables

        internal static List<CoreConsoleEditorWindow> _listOfEditorWindowOfCoreConsole;
        internal static List<CoreConsoleConfiguretionFile> _listOfGameConfiguretorAsset = new List<CoreConsoleConfiguretionFile>();
        internal const float defaultConsoleWidth = 480f;
        internal const float defaultConsoleHeight= 480f;

        internal const float minConsoleHeightRatioForLogList = 0.7f;
        internal const float minConsoleHeightRatioForLogInfo = 0.3f;

        internal const float minHeightOfLogList = defaultConsoleHeight * minConsoleHeightRatioForLogList;
        internal const float minHeightOfLogInfo = defaultConsoleHeight * minConsoleHeightRatioForLogInfo;
        

        [SerializeField] private bool _isClearOnEnteringPlayMode;
        [SerializeField] private bool _isClearOnBuild;

        [SerializeField] private bool _errorPause;
        [SerializeField] private bool _showPrefix;
        [SerializeField] private bool _showTimeStamp;


        [SerializeField] private bool _enableInfoLog = true;
        [SerializeField] private bool _enableLogWarning = true;
        [SerializeField] private bool _enableLogError = true;

        [SerializeField] private float _heightOfLogList;
        [SerializeField] private float _heightOfLogInfo;

        [SerializeField] private bool[] _gameConfiguretorEnableStatus;
        [SerializeField] private string[] _gameConfiguretorOptionLabels;

        private CoreConsoleEditorWindow _editorWindowOfCoreConsole;

        private GUIContent _GUIContentForClearDropdownButton = new GUIContent();

        private GUIContent _GUIContentForCoreConsoleInterfaceSettings = new GUIContent();

        private GUIContent _GUIContentForSelectedCoreConsoleAsset = new GUIContent();

        private GUIContent _GUIContentForTogglingInfoLog = new GUIContent();
        private GUIContent _GUIContentForTogglingWarningLog = new GUIContent();
        private GUIContent _GUIContentForTogglingErrorLog = new GUIContent();

        private GUIContent _GUIContentForInfoLog = new GUIContent();
        private GUIContent _GUIContentForWarningLog = new GUIContent();
        private GUIContent _GUIContentForErrorLog = new GUIContent();

        private GUIContent _GUIContentForLogMessage = new GUIContent();

        private string _searchText = "";

        private float _contentHeightForLogsInList = 30;

        private int     _selectedLogIndex;
        private string  _selectedLogCondition;
        private string  _selectedLogStackTrace;
        private Vector2 _scrollPositionForListOfLog;
        private Vector2 _scrollPositionForLogMessage;

        private Color _selectedLogColor;
        private Color defaultBackgroundColor;
        private Color defaultContentColor;

        

        #endregion

        #region Editor  :   Static

        [MenuItem("FAITH/CoreConsole/New CoreConsole", priority = 0)]
        public static void ShowWindow()
        {

            if (_listOfEditorWindowOfCoreConsole == null)
                _listOfEditorWindowOfCoreConsole = new List<CoreConsoleEditorWindow>();

            CreateInstance<CoreConsoleEditorWindow>().CreateCoreConsole();

        }

        [MenuItem("FAITH/CoreConsole/Close all CoreConsole", priority = 1)]
        public static void CloseAllWindow()
        {

            if (_listOfEditorWindowOfCoreConsole != null)
            {

                int numberOfCoreConsole = _listOfEditorWindowOfCoreConsole.Count;
                for (int i = 0; i < numberOfCoreConsole; i++)
                {

                    _listOfEditorWindowOfCoreConsole[i].Close();
                }
                _listOfEditorWindowOfCoreConsole.Clear();
            }
        }

        #endregion

        #region Editor, Interface

        public override void OnEnable()
        {
            base.OnEnable();

            _editorWindowOfCoreConsole = this;

            if (_listOfEditorWindowOfCoreConsole == null)
                _listOfEditorWindowOfCoreConsole = new List<CoreConsoleEditorWindow>();

            if (!_listOfEditorWindowOfCoreConsole.Contains(this))
                _listOfEditorWindowOfCoreConsole.Add(this);


            Application.logMessageReceivedThreaded += LogMessageReciever;
            EditorApplication.playModeStateChanged += LogPlayModeState;

            UpdateGameConfiguretorAsset();
        }

        public void OnGUI()
        {
            HeaderGUI();

            DrawLogListGUI();

            DrawLogMessageGUI();


        }

        public void OnInspectorUpdate()
        {
            Repaint();

            

        }

        public void OnDisable()
        {
            EditorApplication.playModeStateChanged -= LogPlayModeState;
            Application.logMessageReceivedThreaded -= LogMessageReciever;
        }

        public void OnDestroy()
        {

        }

        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log(_isClearOnBuild);

            if (_isClearOnBuild)
                ClearAllLog();
        }


        private void LogMessageReciever(string condition, string stackTrace, LogType logType)
        {

            if (_errorPause && logType == LogType.Error && !EditorApplication.isPaused)
            {

                bool hasFoundErrorForSelectedLabel = false;
                int numberOfGameConfigAsset = _gameConfiguretorEnableStatus.Length;
                for (int i = 0; i < numberOfGameConfigAsset; i++)
                {

                    if (_gameConfiguretorEnableStatus[i])
                    {

                        if (condition.Contains(_gameConfiguretorOptionLabels[i]))
                        {

                            hasFoundErrorForSelectedLabel = true;
                            break;
                        }
                    }
                }

                if (hasFoundErrorForSelectedLabel)
                {

                    EditorApplication.isPaused = true;
                }


            }


        }

        #endregion

        #region Configuretion

        private bool IsSelectedLog(int logIndex)
        {

            if (_selectedLogIndex != -1 && _selectedLogIndex == logIndex)
                return true;

            return false;
        }

        private int GetNumberOfLog(LogType logType)
        {

            int result = 0;
            int numberOfLogType = _listOfGameConfiguretorAsset.Count;
            for (int i = 0; i < numberOfLogType; i++)
            {

                if (_gameConfiguretorEnableStatus[i])
                    result += _listOfGameConfiguretorAsset[i].GetNumberOfLog(logType);
            }

            return result;
        }

        private string RemoveCoreDebugFromString(string context, CoreConsoleConfiguretionFile gameConfigAsset, LogType logType)
        {

            string result = "";

            if (context.Contains("color"))
            {
                Color color = Color.white;
                switch (logType)
                {

                    case LogType.Log:
                        color = gameConfigAsset.colorForLog;
                        break;

                    case LogType.Warning:
                        color = gameConfigAsset.colorForWarning;
                        break;

                    case LogType.Error:
                        color = gameConfigAsset.colorForLogError;
                        break;
                }
                string hexColor = CoreConsoleUtility.GetHexColorFromRGBColor(color);

                context = context.Replace(string.Format("<color={0}>", hexColor), "");
                context = context.Replace("</color>", "");
            }

            string[] splitBy_ = context.Split('_');
            int numberOfSplit = splitBy_.Length;
            for (int i = 1; i < numberOfSplit; i++)
            {
                result += splitBy_[i];
                result += (i < (numberOfSplit - 1)) ? "_" : "";
            }

            string[] splitByColon = result.Split(':');
            numberOfSplit = splitByColon.Length;
            result = "";
            for (int i = 1; i < numberOfSplit; i++)
            {
                result += splitByColon[i];
            }

            return result;
        }

        private string GetButtonLabeledForGameConfiguretorSelection()
        {

            string result = " ConfigFile ";

            int numberOfSelectedAsset = 0;
            int numberOfGameConfiguretorAsset = _gameConfiguretorEnableStatus.Length;
            for (int i = 0; i < numberOfGameConfiguretorAsset; i++)
            {

                if (_gameConfiguretorEnableStatus[i])
                {

                    result = _gameConfiguretorOptionLabels[i];
                    numberOfSelectedAsset++;
                }
            }

            if (numberOfSelectedAsset > 1)
                result = "Mixed";

            return result;
        }

        private void ClearSelectedIndex()
        {

            _selectedLogIndex = -1;
        }

        private void ClearAllLog()
        {

            foreach (CoreConsoleConfiguretionFile gameConfiguratorAsset in _listOfGameConfiguretorAsset)
                gameConfiguratorAsset.ClearAllLog();
        }

        private void LogPlayModeState(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredPlayMode:

                    if (_isClearOnEnteringPlayMode)
                    {
                        ClearAllLog();
                    }
                    break;
            }
        }

        private void UpdateGameConfiguretorAsset()
        {

            _contentHeightForLogsInList = 30;

            _selectedLogColor = new Color(125 / 255.0f, 195 / 255.0f, 255 / 255.0f, 1f);
            defaultBackgroundColor = GUI.backgroundColor;
            defaultContentColor = GUI.contentColor;

            _GUIContentForClearDropdownButton.image = EditorGUIUtility.IconContent("Icon Dropdown").image;

            _GUIContentForCoreConsoleInterfaceSettings = EditorGUIUtility.TrTextContent("Settings", "Additional Settings for CoreConsoleInterface");

            _GUIContentForTogglingInfoLog.image = EditorGUIUtility.IconContent("console.infoicon.sml").image;
            _GUIContentForTogglingWarningLog.image = EditorGUIUtility.IconContent("console.warnicon.sml").image;
            _GUIContentForTogglingErrorLog.image = EditorGUIUtility.IconContent("console.erroricon.sml").image;

            _GUIContentForInfoLog.image = EditorGUIUtility.IconContent("console.infoicon.sml@2x").image;
            _GUIContentForWarningLog.image = EditorGUIUtility.IconContent("console.warnicon.sml@2x").image;
            _GUIContentForErrorLog.image = EditorGUIUtility.IconContent("console.erroricon.sml@2x").image;


            _listOfGameConfiguretorAsset = CoreConsoleEditorUtility.GetAsset<CoreConsoleConfiguretionFile>();

            int numberOfConfiguretorAsset = _listOfGameConfiguretorAsset.Count;

            if (_gameConfiguretorEnableStatus == null)
                _gameConfiguretorEnableStatus = new bool[0];

            if (_gameConfiguretorOptionLabels == null)
                _gameConfiguretorOptionLabels = new string[0];

            int numberOfGameConfigLabled = _gameConfiguretorEnableStatus.Length;

            if (numberOfConfiguretorAsset != numberOfGameConfigLabled)
            {

                string[] newLabel = new string[numberOfConfiguretorAsset];
                bool[] newEnableStatus = new bool[numberOfConfiguretorAsset];
                for (int i = 0; i < numberOfConfiguretorAsset; i++)
                {

                    string prefix = _listOfGameConfiguretorAsset[i].Prefix;
                    if (string.IsNullOrEmpty(prefix) || string.IsNullOrWhiteSpace(prefix))
                    {
                        prefix = _listOfGameConfiguretorAsset[i].name;
                        CoreConsole.LogWarning("ScriptableObject name is assiged as prefix name as the 'prefix' field was empty : " + prefix);
                    }
                    newLabel[i] = prefix;
                }

                for (int i = 0; i < numberOfConfiguretorAsset; i++)
                {

                    for (int j = 0; j < numberOfGameConfigLabled; j++)
                    {

                        if (CoreConsoleUtility.IsSameString(newLabel[i], _gameConfiguretorOptionLabels[j]))
                        {

                            newEnableStatus[i] = _gameConfiguretorEnableStatus[j];
                            break;
                        }
                    }
                }

                _gameConfiguretorOptionLabels = newLabel;
                _gameConfiguretorEnableStatus = newEnableStatus;
            }

        }

        private void CreateCoreConsole()
        {
            UpdateGameConfiguretorAsset();

            string title = "CoreConsole";

            _editorWindowOfCoreConsole = GetWindow<CoreConsoleEditorWindow>(title, typeof(CoreConsoleEditorWindow));


            _editorWindowOfCoreConsole.titleContent.text = title;
            _editorWindowOfCoreConsole.minSize = new Vector2(defaultConsoleWidth, defaultConsoleHeight);
            _editorWindowOfCoreConsole.Show();

            _listOfEditorWindowOfCoreConsole.Add(_editorWindowOfCoreConsole);

        }

        #endregion

        #region CustomGUI

        private void HeaderGUI()
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            {

                bool clearClicked = false;
                if (CoreConsoleEditorUtility.DropDownToggle(ref clearClicked, new GUIContent() { text = "Clear" }, EditorStyles.toolbarDropDown))
                {

                    GenericMenu genericMenuForClearMode = new GenericMenu();

                    genericMenuForClearMode.AddItem(
                            EditorGUIUtility.TrTextContent("Clear on Play", "Pause the EditorApplication when tracing error"),
                            _isClearOnEnteringPlayMode,
                            () => {
                                _isClearOnEnteringPlayMode = !_isClearOnEnteringPlayMode;
                            });

                    //genericMenuForClearMode.AddItem(
                    //        EditorGUIUtility.TrTextContent("Clear on Build", "Pause the EditorApplication when tracing error"),
                    //        _isClearOnBuild,
                    //        () => {
                    //            _isClearOnBuild = !_isClearOnBuild;
                    //        });

                    genericMenuForClearMode.ShowAsContext();
                }

                if (clearClicked)
                {
                    ClearAllLog();
                }

                EditorGUILayout.Space(5f);
                if (EditorGUILayout.DropdownButton(_GUIContentForCoreConsoleInterfaceSettings, FocusType.Passive, EditorStyles.toolbarDropDown)) {

                    GenericMenu genericMenuForInterfaceSettings = new GenericMenu();

                    genericMenuForInterfaceSettings.AddItem(
                            EditorGUIUtility.TrTextContent("Error Pause", "Pause the EditorApplication when tracing error"),
                            _errorPause,
                            () =>
                            {

                                _errorPause = !_errorPause;
                            }
                        );

                    genericMenuForInterfaceSettings.AddItem(
                            EditorGUIUtility.TrTextContent("Show TimeStamp", "Pause the EditorApplication when tracing error"),
                            _showTimeStamp,
                            () =>
                            {
                                _showTimeStamp = !_showTimeStamp;
                            }
                        );

                    genericMenuForInterfaceSettings.AddItem(
                            EditorGUIUtility.TrTextContent("Show Prefix", "Prefix, that will define the Owner/Source from this log"),
                            _showPrefix,
                            () =>
                            {
                                _showPrefix = !_showPrefix;
                            }
                        );

                    genericMenuForInterfaceSettings.AddSeparator("");

                    genericMenuForInterfaceSettings.AddItem(
                            EditorGUIUtility.TrTextContent("Open CoreConsoleViwer", "Open 'CoreConsoleViwer' to see all the configuretion files"),
                            false,
                            () =>
                            {
                                CoreConsoleViewrEditorWindow.ShowWindow();
                            }
                        );

                    genericMenuForInterfaceSettings.ShowAsContext();
                }

                GUILayout.FlexibleSpace();

                _searchText = EditorGUILayout.TextField(_searchText, EditorStyles.toolbarSearchField);

                EditorGUILayout.Space(5f);
                _GUIContentForSelectedCoreConsoleAsset.text = GetButtonLabeledForGameConfiguretorSelection();
                if (EditorGUILayout.DropdownButton(_GUIContentForSelectedCoreConsoleAsset, FocusType.Passive, EditorStyles.toolbarDropDown))
                {

                    UpdateGameConfiguretorAsset();

                    GenericMenu genericMenuForGameConfiguretorSelection = new GenericMenu();
                    int numberOfOption = _gameConfiguretorOptionLabels.Length;

                    for (int i = 0; i < numberOfOption; i++)
                    {

                        genericMenuForGameConfiguretorSelection.AddItem(
                            new GUIContent(_gameConfiguretorOptionLabels[i]),
                            _gameConfiguretorEnableStatus[i],
                            (index) => {
                                int selectedIndex = (int)index;

                                if (!_gameConfiguretorEnableStatus[selectedIndex] == true)
                                {
                                    //if : Requested To Be True
                                    SerializedObject _soGameConfiguretorAsset = new SerializedObject(_listOfGameConfiguretorAsset[selectedIndex]);
                                    SerializedProperty _spEnableStackTrace = _soGameConfiguretorAsset.FindProperty("_enableStackTrace");

                                    if (!_spEnableStackTrace.boolValue)
                                    {
                                        // if : StackTrace is Disabled
                                        bool result = EditorUtility.DisplayDialog(
                                        "Enable StackTrace",
                                        "In order store and display the logs in 'CoreConsole', 'StackTrace' need to be enabled from 'GameConfiguretionAsset'",
                                        "Enable", "Cancel");

                                        _spEnableStackTrace.boolValue = result;
                                        _spEnableStackTrace.serializedObject.ApplyModifiedProperties();

                                        _gameConfiguretorEnableStatus[selectedIndex] = result;
                                    }
                                    else
                                    {

                                        _gameConfiguretorEnableStatus[selectedIndex] = !_gameConfiguretorEnableStatus[selectedIndex];
                                    }
                                }
                                else
                                {

                                    _gameConfiguretorEnableStatus[selectedIndex] = !_gameConfiguretorEnableStatus[selectedIndex];
                                }

                                ClearSelectedIndex();
                            },
                            i);
                    }

                    if (numberOfOption == 0) {

                        genericMenuForGameConfiguretorSelection.AddItem(
                                EditorGUIUtility.TrTextContent("Create DefaultSettings", "Create 'DefaultSettings' for CoreConsole"),
                                false,
                                () =>
                                {
                                    CoreConsoleViewrEditorWindow.ShowWindow();
                                }
                            );
                    }

                    genericMenuForGameConfiguretorSelection.ShowAsContext();
                }

                //InfoLog
                DrawToggolingLogsGUI(LogType.Log);

                //WarningLog
                DrawToggolingLogsGUI(LogType.Warning);

                //ErrorLog
                DrawToggolingLogsGUI(LogType.Error);
            }
            EditorGUILayout.EndHorizontal();

        }

        private void DrawToggolingLogsGUI(LogType logType)
        {
            Color defaultBackgroundColorOfGUI = GUI.backgroundColor;
            Color dynamicColor = defaultBackgroundColorOfGUI;
            float baseWidth = 15;
            switch (logType)
            {

                case LogType.Log:

                    _GUIContentForTogglingInfoLog.text = GetNumberOfLog(LogType.Log).ToString();
                    Vector2 sizeForInfoLogs = GUI.skin.label.CalcSize(_GUIContentForTogglingInfoLog);

                    dynamicColor.a = _enableInfoLog ? 1f : 0.5f;
                    GUI.backgroundColor = dynamicColor;
                    if (GUILayout.Button(_GUIContentForTogglingInfoLog, GUILayout.Width(baseWidth + sizeForInfoLogs.x)))
                    {
                        _enableInfoLog = !_enableInfoLog;
                        ClearSelectedIndex();
                    }
                    GUI.backgroundColor = defaultBackgroundColorOfGUI;

                    break;

                case LogType.Warning:

                    _GUIContentForTogglingWarningLog.text = GetNumberOfLog(LogType.Warning).ToString();
                    Vector2 sizeForWarningLog = GUI.skin.label.CalcSize(_GUIContentForTogglingWarningLog);

                    dynamicColor.a = _enableLogWarning ? 1f : 0.5f;
                    GUI.backgroundColor = dynamicColor;
                    if (GUILayout.Button(_GUIContentForTogglingWarningLog, GUILayout.Width(baseWidth + sizeForWarningLog.x)))
                    {
                        _enableLogWarning = !_enableLogWarning;
                        ClearSelectedIndex();
                    }
                    GUI.backgroundColor = defaultBackgroundColorOfGUI;

                    break;

                case LogType.Error:

                    _GUIContentForTogglingErrorLog.text = GetNumberOfLog(LogType.Error).ToString();
                    Vector2 sizeForErrorLog = GUI.skin.label.CalcSize(_GUIContentForTogglingErrorLog);

                    dynamicColor.a = _enableLogError ? 1f : 0.5f;
                    GUI.backgroundColor = dynamicColor;
                    if (GUILayout.Button(_GUIContentForTogglingErrorLog, GUILayout.Width(baseWidth + sizeForErrorLog.x)))
                    {
                        _enableLogError = !_enableLogError;
                        ClearSelectedIndex();
                    }
                    GUI.backgroundColor = defaultBackgroundColorOfGUI;

                    break;
            }
        }

        private void DrawLogListGUI()
        {
            _heightOfLogList = position.height * minConsoleHeightRatioForLogList;
            _heightOfLogList = _heightOfLogList < minConsoleHeightRatioForLogList ? minConsoleHeightRatioForLogList : _heightOfLogList;
            EditorGUILayout.BeginVertical(GUILayout.Height(_heightOfLogList));
            {
                _scrollPositionForListOfLog = EditorGUILayout.BeginScrollView(_scrollPositionForListOfLog);
                {
                    GUIStyle GUIStyleForLogDetail = new GUIStyle(EditorStyles.toolbarButton);
                    GUIStyleForLogDetail.alignment = TextAnchor.MiddleLeft;
                    GUIStyleForLogDetail.fontSize = 12;
                    GUIStyleForLogDetail.fixedHeight = _contentHeightForLogsInList;

                    List<ConsoleDebugInfo> _listOfDebugInfo = new List<ConsoleDebugInfo>();

                    int numberOfGameConfiguretorAsset = _listOfGameConfiguretorAsset.Count;
                    for (int i = 0; i < numberOfGameConfiguretorAsset; i++)
                    {

                        if (_gameConfiguretorEnableStatus[i])
                        {

                            foreach (CoreConsole.DebugInfo debugInfo in _listOfGameConfiguretorAsset[i].EditorListOfLogInfo)
                            {

                                _listOfDebugInfo.Add(new ConsoleDebugInfo()
                                {
                                    gameConfig = _listOfGameConfiguretorAsset[i],
                                    debugInfo = debugInfo
                                });
                            }
                        }
                    }

                    //Sorting
                    int numberOfLog = _listOfDebugInfo.Count;

                    for (int i = 0; i < numberOfLog - 1; i++)
                    {

                        System.DateTime _DataTimeWithWhomeToCompare = System.Convert.ToDateTime(_listOfDebugInfo[i].debugInfo.timeStamp);
                        for (int j = i + 1; j < numberOfLog; j++)
                        {

                            System.DateTime _DataTimeToCompare = System.Convert.ToDateTime(_listOfDebugInfo[j].debugInfo.timeStamp);

                            int compareValue = System.DateTime.Compare(_DataTimeWithWhomeToCompare, _DataTimeToCompare);
                            if (compareValue > 0)
                            {

                                ConsoleDebugInfo tempValue = _listOfDebugInfo[i];
                                _listOfDebugInfo[i] = _listOfDebugInfo[j];
                                _listOfDebugInfo[j] = tempValue;
                            }
                        }
                    }

                    for (int i = 0; i < numberOfLog; i++)
                    {
                        if (string.IsNullOrEmpty(_searchText) || string.IsNullOrWhiteSpace(_searchText))
                            DrawLog(_listOfDebugInfo[i].gameConfig, GUIStyleForLogDetail, _listOfDebugInfo[i].debugInfo, i);
                        else if(_listOfDebugInfo[i].debugInfo.condition.Contains(_searchText)){
                            DrawLog(_listOfDebugInfo[i].gameConfig, GUIStyleForLogDetail, _listOfDebugInfo[i].debugInfo, i);
                        }
                    }

                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawLog(
            CoreConsoleConfiguretionFile gameConfigAsset,
            GUIStyle GUIStyleForLog,
            CoreConsole.DebugInfo debugInfo,
            int logIndex)
        {

            bool show = false;
            Color colorOfContent = defaultContentColor;
            GUIContent GUIContentForLabel = new GUIContent();
            switch (debugInfo.logType)
            {

                case LogType.Log:
                    show = _enableInfoLog;

                    colorOfContent = gameConfigAsset.colorForLog;
                    colorOfContent.a = 1;

                    GUIContentForLabel = _GUIContentForInfoLog;
                    break;

                case LogType.Warning:
                    show = _enableLogWarning;

                    colorOfContent = gameConfigAsset.colorForWarning;
                    colorOfContent.a = 1;

                    GUIContentForLabel = _GUIContentForWarningLog;
                    break;

                case LogType.Error:
                    show = _enableLogError;

                    colorOfContent = gameConfigAsset.colorForLogError;
                    colorOfContent.a = 1;

                    GUIContentForLabel = _GUIContentForErrorLog;
                    break;
            }

            if (show)
            {

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(GUIContentForLabel, GUILayout.Width(_contentHeightForLogsInList), GUILayout.Height(_contentHeightForLogsInList));

                    string condition = RemoveCoreDebugFromString(
                        debugInfo.condition,
                        gameConfigAsset,
                        debugInfo.logType
                        );

                    string finalCondition = "";
                    if (_showTimeStamp)
                        finalCondition += string.Format("[{0}]{1}", debugInfo.timeStamp, _showPrefix ? "__" : ":").ToString();

                    if (_showPrefix)
                        finalCondition += string.Format("[{0}]:", debugInfo.prefix).ToString();

                    finalCondition += condition;

                    GUIStyleForLog.normal.textColor = colorOfContent;
                    GUI.backgroundColor = IsSelectedLog(logIndex) ? _selectedLogColor : defaultBackgroundColor;
                    if (GUILayout.Button(finalCondition, GUIStyleForLog))
                    {
                        _selectedLogIndex = logIndex;
                        _selectedLogCondition = finalCondition;
                        _selectedLogStackTrace = debugInfo.stackTrace;
                    }
                    GUI.backgroundColor = defaultBackgroundColor;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(5f);

            }


        }

        private void DrawLogMessageGUI()
        {
            _heightOfLogInfo = position.height * minConsoleHeightRatioForLogList;
            _heightOfLogInfo = _heightOfLogInfo < minConsoleHeightRatioForLogInfo ? minConsoleHeightRatioForLogInfo : _heightOfLogInfo;


            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(position.width), GUILayout.Height(_heightOfLogInfo));
            {
                
                _scrollPositionForLogMessage = EditorGUILayout.BeginScrollView(_scrollPositionForLogMessage);
                {
                    if (IsSelectedLog(_selectedLogIndex))
                    {

                        _GUIContentForLogMessage.text = CoreConsoleUtility.StacktraceWithHyperlinks(string.Format("{0}\n{1}", _selectedLogCondition, _selectedLogStackTrace));

                        GUIStyle _consoleMessageStyle = new GUIStyle(GUI.skin.label);
                        _consoleMessageStyle.alignment = TextAnchor.UpperLeft;
                        _consoleMessageStyle.wordWrap = true;
                        _consoleMessageStyle.richText = true;

                        float height = _consoleMessageStyle.CalcHeight(_GUIContentForLogMessage, this.position.width);

                        EditorGUILayout.SelectableLabel(_GUIContentForLogMessage.text, _consoleMessageStyle, GUILayout.Height(height));
                    }
                    else
                    {

                        EditorGUILayout.HelpBox("No Valid Log Selected", MessageType.Warning);
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();

        }

        #endregion
    }
}

